﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Antlr4.Runtime
{
    public static class AntlrCsCompatability
    {
        public static int LA(this ICharStream self, int i)
        {
            return self.La(i: i);
        }
    }
}
