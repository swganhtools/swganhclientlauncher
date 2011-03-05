using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientLauncher
{
    internal class MathHelper
    {
        internal static double DegToRad(double degrees)
        {
            return (degrees / 180.0) * Math.PI;
        }
    }
}
