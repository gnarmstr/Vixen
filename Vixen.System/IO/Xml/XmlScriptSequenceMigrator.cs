﻿using System.Xml.Linq;

namespace Vixen.IO.Xml {
	class XmlScriptSequenceMigrator : EmptyMigrator {
		private XElement _content;

		public XmlScriptSequenceMigrator(XElement content) {
			_content = content;
		}
	}
}
