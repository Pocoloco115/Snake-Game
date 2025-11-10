using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Interfaces
{
    internal interface IInputHandler
    {
        void GetDirection();
        enum Directions 
        { 
            R, L, U, D
        }
    }
}
