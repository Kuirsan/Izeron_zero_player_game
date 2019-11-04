using System;
using System.Collections.Generic;
using System.Text;

namespace SomeKindOfGame.Objects
{
    abstract class AbstractMainObject
    {
        private readonly protected string _name;
        public AbstractMainObject(string Name)
        {
            _name = Name;
        }
        public override string ToString()
        {
            return _name;
        }
    }
}
