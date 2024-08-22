/*
 Copyright (C) 2024 Anchur
 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at

 http://www.apache.org/licenses/LICENSE-2.0

 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
*/
using KdbSharp.Extensions;
using KdbSharp.Serialization;
using System.Net.Sockets;
using System.Text;

namespace KdbSharp;

public class KdbConnectionOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5000;
    public string? Username { get; set; } = null;
    public string? Password { get; set; } = null;
    public bool UseCompression { get; set; } = false;
    public Encoding TextEncoding { get; set; } = Encoding.UTF8;
    public int MaxReadingChunk { get; set; } = 0x10000;
    public int MaxWritingChunk { get; set; } = 0x10000;
}

public class KdbConnectionBase
{
    public KdbConnectionOptions Options { get; }
    public Encoding TextEncoding => Options.TextEncoding;

    // The underlying tcp connection and the network stream.
    private TcpClient? _underlyingConnection;
    private TcpClient UnderlyingConnection =>
        _underlyingConnection ?? throw new InvalidOperationException("Connection is not open.");

    private NetworkStream? _stream;
    private NetworkStream Stream =>
        _stream ?? throw new InvalidOperationException("Connection is not open.");

    // Initialized when the connection is opened.
    private KReadBuffer _readBuffer = null!;
    private KWriteBuffer _writeBuffer = null!;

    public byte ProtocolVersion { get; private set; }

    // Constructors
    public KdbConnectionBase(KdbConnectionOptions options)
    {
        Options = options;
        // basic connection options check
        if (Options.Host == null)
        {
            throw new InvalidOperationException("Host is not specified.");
        }
        if (Options.Port == 0)
        {
            throw new InvalidOperationException("Port is not specified.");
        }
    }

    // Connection string ctor
    public KdbConnectionBase(string connectionString) : this(ParseConnectionString(connectionString))
    {
    }

    // ParseConnectionString, case insensitive
    private static KdbConnectionOptions ParseConnectionString(string connectionString)
    {
        var options = new KdbConnectionOptions();
        var parts = connectionString.Split(';');
        foreach (var part in parts)
        {
            var kv = part.Split('=');
            if (kv.Length != 2)
            {
                throw new ArgumentException($"Invalid connection string: {connectionString}");
            }
            var key = kv[0].Trim().ToLower();
            var value = kv[1].Trim();
            switch (key)
            {
                case "host":
                    options.Host = value;
                    break;
                case "port":
                    options.Port = int.Parse(value);
                    break;
                case "username":
                    options.Username = value;
                    break;
                case "password":
                    options.Password = value;
                    break;
                case "usecompression":
                    options.UseCompression = bool.Parse(value);
                    break;
                case "maxreadingchunk":
                    options.MaxReadingChunk = int.Parse(value);
                    break;
                case "maxwritingchunk":
                    options.MaxWritingChunk = int.Parse(value);
                    break;
                default:
                    throw new ArgumentException($"Invalid connection string: {connectionString}");
            }
        }
        return options;
    }
    // Parse connection string if options is null.
    // Open the actual connection.
    // In the future we may add a connection pool.
    // Now we just open a new tcp connection.
    private async Task OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        _underlyingConnection = new TcpClient();
        await _underlyingConnection.ConnectAsync(Options.Host, Options.Port, cancellationToken);
        _stream = UnderlyingConnection.GetStream();
    }
    private void SetupOpenState()
    {
        _readBuffer = new KReadBuffer();
        _writeBuffer = new KWriteBuffer();
    }
    public async Task OpenAsync(CancellationToken cancellationToken = default)
    {
        // setup tcp connection, protocol handshake
        // 1. check if connection is already open
        ThrowIfOpened();
        // 3. lazy setup private state
        SetupOpenState();

        // 2. setup tcp connection
        await OpenConnectionAsync(cancellationToken);

        // 4. protocol handshake
        string credential = Options.Password is null ? $"{Options.Username}" : $"{Options.Username}:{Options.Password}";
        _writeBuffer.WriteString(credential, TextEncoding);
        _writeBuffer.WriteByte(KConstant.ClientProtocolVersion); // The client's capability (maximum supported protocol version).
        _writeBuffer.WriteByte(0); // ASCII NUL
        await Stream.WriteAsync(_writeBuffer.GetWritedMemory(), cancellationToken);
        await Stream.FlushAsync(cancellationToken);
        _writeBuffer.Clear();

        if (await Stream.ReadAsync(_headerRecvBuffer.AsMemory(0, 1), cancellationToken) != 1)
        {
            Close();
            await OpenConnectionAsync(cancellationToken);

            _writeBuffer.WriteNullTerminatedString(credential, TextEncoding);
            await Stream.WriteAsync(_writeBuffer.GetWritedMemory(), cancellationToken);
            await Stream.FlushAsync(cancellationToken);
            _writeBuffer.Clear();
            if (await Stream.ReadAsync(_headerRecvBuffer.AsMemory(0, 1), cancellationToken) != 1)
            {
                throw new KdbConnectionException("Access denied.");
            }
        }

        // The common capability.
        ProtocolVersion = Math.Min(_headerRecvBuffer[0], KConstant.ClientProtocolVersion);
    }

    // Close the connection
    public void Close()
    {
        if (_underlyingConnection != null)
        {
            _underlyingConnection.Close();
            _underlyingConnection = null;
            _stream = null;
        }
    }
    // Ensure connection is not opened
    private void ThrowIfOpened()
    {
        if (_underlyingConnection != null)
        {
            throw new InvalidOperationException("Connection is already open.");
        }
    }
    // Ensure connection is opened
    public void ThrowIfClosed()
    {
        if (_underlyingConnection == null)
        {
            throw new InvalidOperationException("Connection is not open.");
        }
    }

    // Impl of the two funcions.
    // fun SendAsync(KMessage): Task;
    // fun RecvAsync(): Task<KMessage>;
    private readonly byte[] _headerRecvBuffer = new byte[KMessage.HEADER_SIZE];
    public async Task SendAsync(KMessage message, CancellationToken cancellation = default)
    {
        ThrowIfClosed();
        // The connection will corrupt if any exception is thrown here.
        try
        {
            // Serialize message header.
            _writeBuffer.Write(message.HeaderMeta);
            _writeBuffer.WriteInt32(message.Size);
            // Send serialized header and body.
            await Stream.WriteAsync(_writeBuffer.GetWritedMemory(), cancellation);
            await Stream.WriteAsync(message.Body, cancellation);
            await Stream.FlushAsync(cancellation);
            _writeBuffer.Clear();
        }
        catch (Exception)
        {
            Close();
            throw;
        }
    }
    public async Task<KMessage> RecvAsync(CancellationToken cancellation = default)
    {
        ThrowIfClosed();
        try
        {
            await Stream.PopulateMemory(_headerRecvBuffer, cancellationToken: cancellation);
            _readBuffer.SetBuffer(_headerRecvBuffer);

            var headerMeta = _readBuffer.Read<MessageHeaderMeta>();
            _readBuffer.IsLittleEndian = headerMeta.Endianess == Endianess.LittleEndian;
            var messageLen = _readBuffer.ReadInt32();

            var message = KMessage.Alloc(headerMeta, messageLen);
            await Stream.PopulateMemory(message.Body, cancellation);
            return message;
        }
        catch (Exception)
        {
            Close();
            throw;
        }
    }
}
