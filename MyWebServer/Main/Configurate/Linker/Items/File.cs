using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configurate.Users;

namespace Configurate.Resouces.Items
{
    public class LinkFile : IitemRead
    {
        private FileInfo Resource;

		public override string Extension {
			get { return Resource.Extension; }
		}

        public LinkFile(FileInfo inf, IitemRead _parent, params GroupInfo[] valid_groups)
            : base()
        {
            Resource = inf;
            Parent = _parent;
            for (int i = 0; i < valid_groups.Length; i++) {
                Groups.Add(valid_groups[i]);
            }
        }

        public override string GetName() {
            return Resource.Name;
        }

        public override void SetInfo(FileSystemInfo target, IitemRead New_parent)
        {
            if (target is FileInfo) {
                Resource = target as FileInfo;
                Parent = New_parent;
            }
            else {
                throw new FormatException(target.FullName);
            }
        }

		public override Stream GetData() {
            return Resource.OpenRead();
		}

		public override void RemoveThis()
		{
			Parent.Remove(this);
		}
	}
}
