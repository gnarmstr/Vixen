﻿using System.Collections.Generic;
using System.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class EmptyMigrator : IMigrator {
		public IEnumerable<IFileOperationResult> Migrate(int fromVersion, int toVersion) {
			return new FileOperationResult(false, "There is only one version.").AsEnumerable();
		}

		public IEnumerable<MigrationSegment> ValidMigrations {
			get { return Enumerable.Empty<MigrationSegment>(); }
		}
	}
}
