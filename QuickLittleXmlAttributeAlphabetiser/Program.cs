/*
	C# "Program.cs"
	by Jeremy Love
	copyright 2021
*/

using System;
using System.IO;

namespace QuickLittleXmlAttributeAlphabetiser {

    public static class Program {

		public static int Main(String[] arguments) {
			try {
				writeBanner();
				if(0 == arguments.Length) {
					writeUsage();
					return(0);
				}
				Alphabetiser alphabetiser = new Alphabetiser();
				foreach(String argument in arguments) {
					if(File.Exists(argument)) {
						alphabetiser.alphabetiseFile(argument);
						continue;
					}
					if(Directory.Exists(argument)) {
						alphabetiser.alphabetiseDirectory(argument);
						continue;
					}
					Console.Error.WriteLine("Invalid argument: {0}", argument);
				}
				return(0);
			}
			catch(Exception exception) {
				Console.Error.WriteLine("Error: {0}", exception);
				return(1);
			}
		}

		public static void writeBanner() {
			Console.Out.WriteLine("Quick Little Xml Attribute Alphabetiser");
			Console.Out.WriteLine("Copyright 2021 by Jeremy Love");
			Console.Out.WriteLine();
		}

		public static void writeUsage() {
			Console.Out.WriteLine("Usage:");
			Console.Out.WriteLine("\tQuickLittleXmlAttributeAlphabetiser.exe \"path\\to\\File.xml\"");
			Console.Out.WriteLine("or:");
			Console.Out.WriteLine("\tQuickLittleXmlAttributeAlphabetiser.exe \"path\\to\\directory\\\"");
		}

	}

}