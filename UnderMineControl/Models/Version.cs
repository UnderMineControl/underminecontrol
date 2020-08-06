// This is a straight rip from UnderMod
// https://github.com/bwdymods/UnderMod/blob/master/UnderModAPI/Structs/Version.cs
using System;

namespace UnderMineControl.Models
{
    using API.Models;

    public class Version : IComparable, IVersion
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }
        public int Revision { get; set; }

        public Version(int major, int minor = 0, int patch = 0, int revision = 0)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Revision = revision;
        }

        public Version(string versionString)
        {
            string[] v = versionString.Split('.');
            Major = 0;
            if (v.Length > 0) Major = int.Parse(v[0]);
            Minor = 0;
            if (v.Length > 1) Minor = int.Parse(v[1]);
            Patch = 0;
            if (v.Length > 2) Patch = int.Parse(v[2]);
            Revision = 0;
            if (v.Length > 3) Revision = int.Parse(v[3]);
        }

        public static bool operator <(Version emp1, Version emp2)
        {

            return Compare(emp1, emp2) < 0;

        }

        public static bool operator >(Version emp1, Version emp2)
        {

            return Compare(emp1, emp2) > 0;

        }

        public static bool operator ==(Version emp1, Version emp2)
        {

            return Compare(emp1, emp2) == 0;

        }

        public static bool operator !=(Version emp1, Version emp2)
        {

            return Compare(emp1, emp2) != 0;

        }

        public override bool Equals(object obj)
        {

            if (!(obj is Version)) return false;

            return this == (Version)obj;

        }

        public static bool operator <=(Version emp1, Version emp2)
        {

            return Compare(emp1, emp2) <= 0;

        }

        public static bool operator >=(Version emp1, Version emp2)
        {

            return Compare(emp1, emp2) >= 0;

        }

        public static int Compare(Version a, Version b)
        {
            if (a.Major > b.Major) return 1;
            if (a.Major < b.Major) return -1;
            if (a.Minor > b.Minor) return 1;
            if (a.Minor < b.Minor) return -1;
            if (a.Patch > b.Patch) return 1;
            if (a.Patch < b.Patch) return -1;
            if (a.Revision > b.Revision) return 1;
            if (a.Revision < b.Revision) return -1;
            return 0;
        }

        public override string ToString()
        {
            return Major + "." + Minor + "." + Patch + "." + Revision;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is Version)) return 1;
            return Compare(this, (Version)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = -1661427959;
            hashCode = hashCode * -1521134295 + Major.GetHashCode();
            hashCode = hashCode * -1521134295 + Minor.GetHashCode();
            hashCode = hashCode * -1521134295 + Patch.GetHashCode();
            hashCode = hashCode * -1521134295 + Revision.GetHashCode();
            return hashCode;
        }
    }
}
