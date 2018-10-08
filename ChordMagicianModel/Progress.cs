using System;
using System.Collections.Generic;

namespace ChordMagicianModel
{
    public class Progress
    {
        #region Properties

        public string ChildPath { get; set; }

        public string Id { get; }

        public string Html { get; }

        public string Chord { get; set; }

        public List<byte> ChordNotes { get; set; } = new List<byte>();

        #endregion

        private readonly double _probability;

        public Progress(string id, string html, double probability, string childPath)
        {
            Id = id;
            Html = html;
            _probability = probability;
            ChildPath = childPath;
            Chord = "";
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Chord))
            {
                return String.Format("{0}\n({1} %)", Id, _probability * 100);
            }
            else
            {
                return String.Format("{0}\n({1} %)", Chord, _probability * 100);
            }
        }
    }
}
