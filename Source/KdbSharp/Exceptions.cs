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
using KdbSharp.Types;

namespace KdbSharp;


[Serializable]
public class KdbSharpException : Exception
{
    public KdbSharpException() { }
    public KdbSharpException(string message) : base(message) { }
    public KdbSharpException(string message, Exception inner) : base(message, inner) { }
    protected KdbSharpException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}


[Serializable]
public class KdbException : Exception
{
    public KdbException() { }
    public KdbException(string message) : base(message) { }
    public KdbException(string message, Exception inner) : base(message, inner) { }
    protected KdbException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    public static Dictionary<string, string> KdbErrorMessageDictionary { get; } = new Dictionary<string, string>()
    {
        {"nyi", "Not yet implemented" }
    };

    public static KdbException CreateFromKdbErrorMessage(string errorMessage)
    {
        var message = errorMessage;
        if (KdbErrorMessageDictionary.TryGetValue(errorMessage, out var description))
        {
            message = $"{errorMessage}; {description}";
        }
        return new KdbException(message);
    }
}

public class KdbConnectionException : Exception
{
    public KdbConnectionException(string message) : base(message)
    {
    }
}
// protocol error
public class KdbProtocolException : Exception
{
    public KdbProtocolException() { }
    public KdbProtocolException(string message) : base(message) { }
    public KdbProtocolException(string message, Exception inner) : base(message, inner) { }
    protected KdbProtocolException(
              System.Runtime.Serialization.SerializationInfo info,
                       System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    public static string UncompressionError => "Error while data uncompression.";
}

[Serializable]
public class MessageSerializationException : Exception
{
    public MessageSerializationException() { }
    public MessageSerializationException(string message) : base(message) { }
    public MessageSerializationException(string message, Exception inner) : base(message, inner) { }
    protected MessageSerializationException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

public static class ThrowHelper
{
    // Conversion not support
    public static void ThrowConversionNotSupport(Type t, KType kt)
    {
        throw new KdbSharpException($"Conversion between {t} and {kt} is not supported.");
    }

    internal static void ThrowNoDefaultConversionForType(Type type)
    {
        throw new NotImplementedException();
    }
}
