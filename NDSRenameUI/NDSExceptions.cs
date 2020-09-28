using System;

namespace NDSRenameUI
{
    public class NDSDatabaseLoadException : Exception
    {
        public NDSDatabaseLoadException()
        {
        }

        public NDSDatabaseLoadException(string message)
            : base(message)
        {
        }
    }
    public class NDSFileStreamException : Exception
    {
        public NDSFileStreamException()
        {
        }

        public NDSFileStreamException(string message)
            : base(message)
        {
        }
    }

    public class NDSFileException : Exception
    {
        public NDSFileException()
        {
        }

        public NDSFileException(string message)
            : base(message)
        {
        }
    }
}
