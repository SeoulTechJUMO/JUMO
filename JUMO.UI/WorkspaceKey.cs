﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO.UI
{
    public abstract class WorkspaceKey
    {
        protected object data;

        public WorkspaceKey(object data) => this.data = data;

        public abstract override bool Equals(object obj);
        public override int GetHashCode() => base.GetHashCode();
    }

    public class PianoRollWorkspaceKey : WorkspaceKey
    {
        public PianoRollWorkspaceKey(Vst.Plugin plugin) : base(plugin) { }

        public override bool Equals(object obj)
        {
            if (!(obj is PianoRollWorkspaceKey key))
            {
                return false;
            }

            return ReferenceEquals(data, key.data);
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}
