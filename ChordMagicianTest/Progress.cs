using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ChordMagicianTest
{
    public class Progress
    {
        private string _ID;
        private string _HTML;
        private double _Probability;
        private string _Child_path;

        private string _Chord;

        public override string ToString()
        {
            if (_Chord == "")
            {
                return String.Format("({0})\n({1} %) {2}", _ID, _Probability * 100, _HTML);
            }
            else
            {
                return String.Format("{0}\n({1} %)", _Chord, _Probability * 100);
            }
        }

        public Progress(string id, string html, double probability, string child_path)
        {
            _ID = id;
            _HTML = html;
            _Probability = probability;
            _Child_path = child_path;
            _Chord = "";
        }

        public string ChildPath {
            get => _Child_path;
            set
            {
                _Child_path = value;
            }
        }

        public string ID
        {
            get => _ID;
        }

        public string Chord
        {
            get => _Chord;
            set
            {
                _Chord = value;
            }
        }
    }
}
