/*
* FILE : ClientStatus.cs
* PROJECT : PROG2121-Windows and Mobile Programming - Assignment #5
* PROGRAMMER : Gursharan Singh - Waqar Ali Saleemi
* FIRST VERSION : 2012-11-12
* DESCRIPTION :
* Model class for to store client status
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public static class ClientStatus
    {
        public static string Guid { get; set; }

        public static string Name { get; set; }

        public static int MaxRange { get; set; }

        public static int MinRange { get; set; }

        public static string serverIP { get; set; }

        public static int port { get; set; }


    }
}
