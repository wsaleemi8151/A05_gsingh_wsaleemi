/*
* FILE : GameClient.cs
* PROJECT : PROG2121-Windows and Mobile Programming - Assignment #5
* PROGRAMMER : Gursharan Singh - Waqar Ali Saleemi
* FIRST VERSION : 2012-11-12
* DESCRIPTION :
* Model class for Game Client which holds client session
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class GameClient
    {
        public string Guid { get; set; }

        public int MaxRange { get; set; }

        public int MinRange { get; set; }

        public int TheNumber { get; set; }
    }
}
