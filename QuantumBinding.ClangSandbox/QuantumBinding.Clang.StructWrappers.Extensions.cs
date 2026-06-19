using System;
using System.Collections.Generic;
using System.Text;

namespace QuantumBinding.Clang
{
    public partial class QBString
    {
        public override string ToString()
        {
            string retval = GetCString();
            //DisposeString();
            return retval;
        }
    }

    public partial class QBType
    {
        public override string ToString()
        {
            return GetTypeSpelling().ToString();
        }
    }

    public partial class QBCursor
    {
        public override string ToString()
        {
            return GetCursorSpelling().ToString();
        }
    }

    public partial class QBDiagnostic
    {
        public override string ToString()
        {
            return GetDiagnosticSpelling().ToString();
        }
    }
}
