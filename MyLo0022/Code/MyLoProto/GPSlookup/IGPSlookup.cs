/*

Copyright Keith Short 2012
keith_short@hotmail.com

This file is part of the MyLo application

------------------------------------------------------------------------
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPSlookupNS
{
    public interface IGPSlookup
    {
        void LatLongToAddressLookup(double latitude, double longitude, out string street, out string city, out string state, out string zip, out string country);

        void AddressToLatLongLookup(string street, string city, string state, string zip, string countrydouble, out double latitude, out double longitude);
    }
}
