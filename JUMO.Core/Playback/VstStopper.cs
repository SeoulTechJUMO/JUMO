using System.Collections.Generic;

namespace JUMO.Playback
{
    class VstStopper
    {
        private Dictionary<Vst.Plugin, bool[]> _maskTable = new Dictionary<Vst.Plugin, bool[]>();

        private bool[] this[Vst.Plugin plugin]
        {
            get
            {
                if (_maskTable.TryGetValue(plugin, out bool[] noteMask))
                {
                    return noteMask;
                }
                else
                {
                    bool[] newNoteMask = new bool[128];

                    _maskTable.Add(plugin, newNoteMask);

                    return newNoteMask;
                }
            }
        }

        public void MarkNoteOn(Vst.Plugin plugin, byte value) => this[plugin][value] = true;

        public void MarkNoteOff(Vst.Plugin plugin, byte value) => this[plugin][value] = false;

        public void StopAllSound()
        {
            foreach (var item in _maskTable)
            {
                Vst.Plugin plugin = item.Key;
                bool[] mask = item.Value;

                for (byte i = 0; i < 128; i++)
                {
                    if (mask[i])
                    {
                        plugin.NoteOff(i);

                        mask[i] = false;
                    }
                }
            }
        }
    }
}
