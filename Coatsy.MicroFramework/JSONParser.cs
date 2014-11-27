﻿using System;
using System.Collections;
using System.IO;
using Microsoft.SPOT;

namespace netduino.helpers.Helpers
{
    // From http://netduinohelpers.codeplex.com/SourceControl/latest
    public class JSONParser : IDisposable
    {
        private Char[] _accumulator;
        private Char[] _unicodeAccumulator;
        private Stack _dataStack;
        private Stack _dataStructureStack;
        private JSONObject _currentDataStructure;
        private string _dataStructureName;
        private bool _inString;
        private bool _inStringSlash;
        private bool _inValue;
        private bool _inUnicodeCharacter;
        private bool _okToPushEmptyValue;
        private bool _IgnoreNextComma;
        private bool _IgnoreNextDataPush;
        private int _accumulatorIndex;
        private int _unicodeCharacterIndex;

        public JSONParser(int maxStringCapacity = 512)
        {
            AutoCasting = true;
            _accumulator = new Char[maxStringCapacity + 1];
            _unicodeAccumulator = new Char[4];
            _dataStack = new Stack();
            _dataStructureStack = new Stack();
        }

        public bool AutoCasting { get; set; }

        public Hashtable Parse(StreamReader stream)
        {
            var length = stream.BaseStream.Length;
            while (length-- > 0)
            {
                ProcessCharacter((Char)stream.Read());
            }
            var arrayList = (ArrayList)(_currentDataStructure.Object);
            return (Hashtable)arrayList[0];
        }

        public Hashtable Parse(string jsonText)
        {
            foreach (Char c in jsonText)
            {
                ProcessCharacter(c);
            }
            var arrayList = (ArrayList)(_currentDataStructure.Object);
            return (Hashtable)arrayList[0];
        }

        protected void ProcessCharacter(Char c)
        {
            switch (c)
            {
                case '{':
                    PushDataStructure(new Hashtable(), JSONObjectType.Object);
                    _okToPushEmptyValue = true;
                    _IgnoreNextComma = false;
                    _IgnoreNextDataPush = false;
                    return;
                case '}':
                    StoreDataStructure();
                    _inValue = false;
                    _okToPushEmptyValue = false;
                    _IgnoreNextComma = true;
                    return;
                case '[':
                    PushDataStructure(new ArrayList(), JSONObjectType.Array);
                    _okToPushEmptyValue = true;
                    _IgnoreNextComma = false;
                    _IgnoreNextDataPush = false;
                    return;
                case ']':
                    StoreDataStructure();
                    _inValue = false;
                    _okToPushEmptyValue = false;
                    _IgnoreNextComma = true;
                    return;
                case '"':
                    if (!_inString)
                    {
                        _inString = true;
                    }
                    else
                    {
                        if (_inStringSlash)
                        {
                            Accumulate(c);
                            _inStringSlash = false;
                        }
                        else
                        {
                            PushData(_accumulator);
                            _okToPushEmptyValue = false;
                            _IgnoreNextDataPush = true;
                            _inString = false;
                        }
                    }
                    return;
                case ':':
                    if (!_inString)
                    {
                        _inValue = true;
                        _okToPushEmptyValue = true;
                        _IgnoreNextDataPush = false;
                        return;
                    }
                    break;
                case ',':
                    if (_IgnoreNextComma)
                    {
                        _IgnoreNextComma = false;
                        return;
                    }
                    if (!_inString)
                    {
                        PushData(_accumulator);
                        StoreDataInCurrentDataStructure();
                        if (_currentDataStructure.ObjectType == JSONObjectType.Array)
                        {
                            _inValue = true;
                        }
                        else
                        {
                            _inValue = false;
                        }
                        return;
                    }
                    break;
                case '\\':
                    if (_inString)
                    {
                        if (_inUnicodeCharacter)
                        {
                            Accumulate(ConvertHexDigit());
                            _inUnicodeCharacter = false;
                        }
                        if (_inStringSlash)
                        {
                            Accumulate(c);
                            _inStringSlash = false;
                        }
                        else
                        {
                            _inStringSlash = true;
                        }
                    }
                    return;
                case 'b':
                    if (_inString)
                    {
                        if (_inStringSlash)
                        {
                            Accumulate('\b');
                            _inStringSlash = false;
                            return;
                        }
                    }
                    break;
                case 'f':
                    if (_inString)
                    {
                        if (_inStringSlash)
                        {
                            Accumulate('\f');
                            _inStringSlash = false;
                            return;
                        }
                    }
                    break;
                case 'n':
                    if (_inString)
                    {
                        if (_inStringSlash)
                        {
                            Accumulate('\n');
                            _inStringSlash = false;
                            return;
                        }
                    }
                    break;
                case 'r':
                    if (_inString)
                    {
                        if (_inStringSlash)
                        {
                            Accumulate('\r');
                            _inStringSlash = false;
                            return;
                        }
                    }
                    break;
                case 't':
                    if (_inString)
                    {
                        if (_inStringSlash)
                        {
                            Accumulate('\t');
                            _inStringSlash = false;
                            return;
                        }
                    }
                    break;
                case 'u':
                    if (_inString)
                    {
                        if (_inStringSlash)
                        {
                            _inUnicodeCharacter = true;
                            _inStringSlash = false;
                            return;
                        }
                    }
                    break;
                case '/':
                    if (_inString)
                    {
                        if (_inStringSlash)
                        {
                            Accumulate(c);
                            _inStringSlash = false;
                            return;
                        }
                    }
                    break;
                case ' ':
                    if (!_inString)
                    {
                        return;
                    }
                    break;
                case '\b':
                    if (!_inString)
                    {
                        return;
                    }
                    break;
                case '\f':
                    if (!_inString)
                    {
                        return;
                    }
                    break;
                case '\n':
                    if (!_inString)
                    {
                        return;
                    }
                    break;
                case '\r':
                    if (!_inString)
                    {
                        return;
                    }
                    break;
                case '\t':
                    if (!_inString)
                    {
                        return;
                    }
                    break;
            }

            if (_inValue || _inString)
            {
                if (_inUnicodeCharacter)
                {
                    AccumulateUnicodeCharacter(c);
                }
                else
                {
                    Accumulate(c);
                }
            }
        }
        protected void PushDataStructure(Object obj, JSONObjectType type)
        {
            _currentDataStructure = new JSONObject(obj, type);
            if (_inValue)
            {
                _currentDataStructure.Name = _dataStructureName;
            }
            _dataStructureStack.Push(_currentDataStructure);
        }
        protected JSONObject PopDataStructure()
        {
            _currentDataStructure = (JSONObject)_dataStructureStack.Pop();
            return _currentDataStructure;
        }
        protected void StoreDataStructure()
        {
            PushData(_accumulator);
            StoreDataInCurrentDataStructure();
            StoreCurrentDataStructure();
        }
        protected void StoreDataInCurrentDataStructure()
        {
            if (_currentDataStructure.ObjectType == JSONObjectType.Object)
            {
                var value = (string)_dataStack.Pop();
                var name = (string)_dataStack.Pop();
                Debug.Print("DAT: Popped pair: " + name + " = " + value);
                var hashTable = (Hashtable)_currentDataStructure.Object;
                hashTable.Add(name, value);
                return;
            }
            else
            {
                var value = (string)_dataStack.Pop();
                Debug.Print("DAT: Popped value: " + value);
                var arrayList = (ArrayList)_currentDataStructure.Object;
                arrayList.Add(value);
            }
        }
        protected void StoreCurrentDataStructure()
        {
            var innerStructure = (JSONObject)_dataStructureStack.Pop();
            _currentDataStructure = (JSONObject)_dataStructureStack.Peek();
            if (_currentDataStructure.ObjectType == JSONObjectType.Object)
            {
                var name = (string)_dataStack.Pop();
                Debug.Print("STR: Popped data: " + name);
                var hashTable = (Hashtable)_currentDataStructure.Object;
                hashTable.Add(name, innerStructure);
            }
            else
            {
                var arrayList = (ArrayList)_currentDataStructure.Object;
                arrayList.Add(innerStructure);
            }
        }
        protected void PushData(Char[] characters)
        {
            if (_IgnoreNextDataPush)
            {
                _IgnoreNextDataPush = false;
                return;
            }
            if (_accumulatorIndex > 0)
            {
                _accumulator[_accumulatorIndex] = (Char)0;
                var accumulatedString = new string(characters, 0, _accumulatorIndex);
                _dataStructureName = accumulatedString;
                //Debug.Print(accumulatedString);
                _dataStack.Push(accumulatedString);
                _accumulatorIndex = 0;
            }
            else
            {
                if (_okToPushEmptyValue)
                {
                    _dataStack.Push("null");
                }
            }
        }
        protected Object AutoCast(string data)
        {
            if (data.ToLower() == "null")
            {
                return null;
            }
            if (data.ToLower() == "true")
            {
                return true;
            }
            if (data.ToLower() == "false")
            {
                return false;
            }
            if (IsNumeric(data))
            {
                try
                {
                    var numericValue = Double.Parse(data);
                    return numericValue;
                }
                catch (Exception)
                {
                    // Do nothing
                }
            }
            return data;
        }
        protected bool IsNumeric(string data)
        {
            foreach (Char c in data)
            {
                if (c >= '0' && c <= '9' || c == '-' || c == '+' || c == 'E' || c == 'e' || c == '.')
                {
                    continue;
                }
                return false;
            }
            return true;
        }
        protected void Accumulate(Char c)
        {
            if (_accumulatorIndex < _accumulator.Length)
            {
                _accumulator[_accumulatorIndex++] = c;
            }
            else
            {
                throw new ArgumentOutOfRangeException("c");
            }
        }
        protected void AccumulateUnicodeCharacter(Char c)
        {
            if (_unicodeCharacterIndex < _unicodeAccumulator.Length)
            {
                _unicodeAccumulator[_unicodeCharacterIndex++] = c;
                if (_unicodeCharacterIndex == _unicodeAccumulator.Length)
                {
                    Accumulate(ConvertHexDigit());
                    _inUnicodeCharacter = false;
                    _unicodeCharacterIndex = 0;
                }
            }
            else throw new ArgumentOutOfRangeException("c");
        }
        public bool Find(string key, Hashtable hashTable, out Hashtable searchResult)
        {
            if (hashTable.Contains(key))
            {
                searchResult = (Hashtable)hashTable[key];
                return true;
            }
            searchResult = null;
            return false;
        }

        public bool Find(string name, Hashtable hashTable, out ArrayList searchResult)
        {
            if (hashTable.Contains(name))
            {
                searchResult = (ArrayList)hashTable[name];
                return true;
            }
            searchResult = null;
            return false;
        }
        public bool Find(string key, Hashtable hashTable, out string searchResult)
        {
            if (hashTable.Contains(key))
            {
                searchResult = (string)hashTable[key];
                return true;
            }
            searchResult = null;
            return false;
        }
        public bool Find(string key, Hashtable hashTable, out bool searchResult)
        {
            if (hashTable.Contains(key))
            {
                if (AutoCasting)
                {
                    searchResult = (bool)hashTable[key];
                    return true;
                }
                var result = (string)hashTable[key];
                if (result == "true")
                {
                    searchResult = true;
                    return true;
                }
                if (result == "false")
                {
                    searchResult = false;
                    return true;
                }
            }
            searchResult = false;
            return false;
        }
        public bool Find(string key, Hashtable hashTable, out Double searchResult)
        {
            if (hashTable.Contains(key))
            {
                if (AutoCasting)
                {
                    searchResult = (Double)hashTable[key];
                }
                else
                {
                    searchResult = Double.Parse((string)hashTable[key]);
                }
                return true;
            }
            searchResult = 0.0;
            return false;
        }
        public bool Find(string key, Hashtable hashTable, out float searchResult)
        {
            if (hashTable.Contains(key))
            {
                if (AutoCasting)
                {
                    searchResult = (float)((Double)hashTable[key]);
                }
                else
                {
                    searchResult = (float)Double.Parse((string)hashTable[key]);
                }
                return true;
            }
            searchResult = 0.0f;
            return false;
        }
        public bool Find(string key, Hashtable hashTable, out Int16 searchResult)
        {
            if (hashTable.Contains(key))
            {
                if (AutoCasting)
                {
                    searchResult = (Int16)((Double)hashTable[key]);
                }
                else
                {
                    searchResult = Int16.Parse((string)hashTable[key]);
                }
                return true;
            }
            searchResult = 0;
            return false;
        }
        public bool Find(string key, Hashtable hashTable, out Int32 searchResult)
        {
            if (hashTable.Contains(key))
            {
                if (AutoCasting)
                {
                    searchResult = (Int32)((Double)hashTable[key]);
                }
                else
                {
                    searchResult = Int32.Parse((string)hashTable[key]);
                }
                return true;
            }
            searchResult = 0;
            return false;
        }
        public bool Find(string key, Hashtable hashTable, out Int64 searchResult)
        {
            if (hashTable.Contains(key))
            {
                if (AutoCasting)
                {
                    searchResult = (Int64)((Double)hashTable[key]);
                }
                else
                {
                    searchResult = Int64.Parse((string)hashTable[key]);
                }
                return true;
            }
            searchResult = 0;
            return false;
        }
        public bool Find(string key, Hashtable hashTable, out UInt16 searchResult)
        {
            if (hashTable.Contains(key))
            {
                if (AutoCasting)
                {
                    searchResult = (UInt16)((Double)hashTable[key]);
                }
                else
                {
                    searchResult = UInt16.Parse((string)hashTable[key]);
                }
                return true;
            }
            searchResult = 0;
            return false;
        }
        public bool Find(string key, Hashtable hashTable, out UInt32 searchResult)
        {
            if (hashTable.Contains(key))
            {
                if (AutoCasting)
                {
                    searchResult = (UInt32)((Double)hashTable[key]);
                }
                else
                {
                    searchResult = UInt32.Parse((string)hashTable[key]);
                }
                return true;
            }
            searchResult = 0;
            return false;
        }
        public bool Find(string key, Hashtable hashTable, out UInt64 searchResult)
        {
            if (hashTable.Contains(key))
            {
                if (AutoCasting)
                {
                    searchResult = (UInt64)((Double)hashTable[key]);
                }
                else
                {
                    searchResult = UInt64.Parse((string)hashTable[key]);
                }
                return true;
            }
            searchResult = 0;
            return false;
        }
        protected Char ConvertHexDigit()
        {
            short hexCharValue = 0;
            int index = 0;
            while (index < _unicodeCharacterIndex)
            {
                hexCharValue <<= 4;
                Char tempChar = _unicodeAccumulator[index++];
                if (tempChar >= '0' && tempChar <= '9')
                {
                    hexCharValue |= (short)(tempChar - '0');
                }
                else if (tempChar >= 'a' && tempChar <= 'f')
                {
                    hexCharValue |= (short)((tempChar - 'a') + 10);
                }
                else if (tempChar >= 'A' && tempChar <= 'F')
                {
                    hexCharValue |= (short)((tempChar - 'A') + 10);
                }
                else
                {
                    throw new IndexOutOfRangeException("tempChar");
                }
            }
            _unicodeCharacterIndex = 0;
            return (Char)hexCharValue;
        }
        public void Dispose()
        {
            _dataStructureStack = null;
            _dataStack = null;
            _accumulator = null;
            _unicodeAccumulator = null;
        }
    }
}


