﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Host.Users;

namespace Resouces
{
    public class LinkFile : IItem
    {
        private FileInfo Resource;
        public IItem Parent;

		public LinkFile(FileInfo inf, IItem _parent, params GroupInfo[] valid_groups) : base() {
            Resource = inf;
            Parent = _parent;
            for (int i = 0; i < valid_groups.Length; i++) {
                Groups.Add(valid_groups[i]);
            }
        }

        public override string GetName() {
            return Resource.Name;
        }

        public override IItem GetParent() {
            return Parent;
        }

        public override FileSystemInfo GetInfo() {
            return Resource;
        }


        public override string GetPath() {
            IItem i = this;
            string res = "";
            while (i.GetParent() != null) {
                res = "/" + i.GetName() + res;
                i = i.GetParent();
            }
            return res;
        }

        public override void SetInfo(FileSystemInfo target, IItem New_parent) {
            if (target is FileInfo) {
                Resource = target as FileInfo;
                Parent = New_parent;
            }
            else {
                throw new FormatException(target.FullName);
            }
        }
	}
}
