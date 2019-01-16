namespace VersioningMMRB.VersionInterface
{
	public class Version
	{
		public int Major { get; private set; }
		public int Minor { get; private set; }
		public int Release { get; private set; }
		public int Build { get; private set; }
		public Version() : this(0, 0, 0, 1)
		{
		}
		public Version(int major, int minor, int release, int build)
		{
			Major = major;
			Minor = minor;
			Release = release;
			Build = build;
		}
		public Version(string version) : this(0, 0, 0, 0)
		{
			if (string.IsNullOrWhiteSpace(version)) { return; }
			string[] split = version.Trim().Split('.');
			if(int.TryParse(split[0], out int major))
			{
				Major = major;
			}
			if(split.Length > 1 && int.TryParse(split[1], out int minor))
			{
				Minor = minor;
			}
			if(split.Length > 2 && int.TryParse(split[2], out int release))
			{
				Release = release;
			}
			if(split.Length > 3 && int.TryParse(split[3], out int build))
			{
				Build = build;
			}
		}
		public void IncrementMajor()
		{
			++Major;
			Minor = 0;
			Release = 0;
			Build = 0;
		}
		public void IncrementMinor()
		{
			++Minor;
			Release = 0;
			Build = 0;
		}
		public void IncrementRelease()
		{
			++Release;
			Build = 0;
		}
		public void IncrementBuild()
		{
			++Build;
		}
		public override string ToString()
		{
			return $"{Major}.{Minor}.{Release}.{Build}";
		}
		public override bool Equals(object obj)
		{
			return this.ToString() == obj.ToString();
		}
		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}
	}
}
