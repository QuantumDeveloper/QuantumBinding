using System;
using System.Collections.Generic;
using System.Text;

namespace QuantumBinding.Clang
{
    public partial class QBString
    {
        public override string ToString()
        {
            string retval = getCString();
            //disposeString();
            return retval;
        }
    }

    public partial class QBType
    {
        public override string ToString()
        {
            return getTypeSpelling().ToString();
        }
    }

    public partial class QBCursor
    {
        public override string ToString()
        {
            return getCursorSpelling().ToString();
        }
    }

    public partial class QBDiagnostic
    {
        public override string ToString()
        {
            return getDiagnosticSpelling().ToString();
        }
    }
}
