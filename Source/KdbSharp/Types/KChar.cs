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
namespace KdbSharp.Types;

public readonly struct KChar : INullable, IEquatable<KChar>
{
    private readonly sbyte _value;

    public static readonly KChar Null = new KChar((sbyte)' ');

    public KChar()
    {
        _value = 0;
    }

    public KChar(sbyte value)
    {
        _value = value;
    }

    public bool IsNull => Equals(Null);

    public sbyte Value => _value;

    public bool Equals(KChar other) => _value == other._value;

    public override string ToString()
    {
        return ((char)_value).ToString();
    }

    public override bool Equals(object? obj)
    {
        return obj is KChar c && Equals(c);
    }

    public static bool operator ==(KChar left, KChar right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(KChar left, KChar right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }
}
