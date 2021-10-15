/*
	C# "Alphabetiser.cs"
	by Jeremy Love
	copyright 2021
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace QuickLittleXmlAttributeAlphabetiser {

	public class Alphabetiser {

		public void alphabetiseDirectory(String directoryPath, String filter = "*.xml") {
			foreach(String filePath in Directory.EnumerateFiles(directoryPath, filter, SearchOption.AllDirectories)) {
				alphabetiseFile(filePath);
			}
		}

		public void alphabetiseFile(String filePath) {
			try {
				Console.Out.WriteLine("Loading: {0}", filePath);
				String fileText = File.ReadAllText(filePath);
				int changeCount;
				fileText = alphabetiseText(fileText, out changeCount);
				if(changeCount > 0) {
					Console.Out.WriteLine("Saving: {0}", filePath);
					File.WriteAllText(filePath, fileText);
				}
			}
			catch(Exception exception) {
				// NOTE: just because work on this file failed, don't stop the program
				Console.Error.WriteLine("Error: {0}", exception);
			}
		}

		private String alphabetiseText(String text, out int outChangeCount) {
			int changeCount = 0;
			StringBuilder stringBuilder = new StringBuilder();
			text = regex.Replace(text, match => {
				Group attributes = match.Groups["attribute"];
				if(0 == attributes.Captures.Count) {
					return(match.Value);
				}
				Group attributeNames = match.Groups["attributeName"];
				Group elementName = match.Groups["elementName"];
				Group elementTail = match.Groups["elementTail"];
				stringBuilder.Clear();
				stringBuilder.Append('<');
				{
					stringBuilder.Append(elementName);
					foreach(String attribute in alphabetiseAttributes(attributes, attributeNames)) {
						stringBuilder.Append(attribute);
					}
					stringBuilder.Append(elementTail);
				}
				stringBuilder.Append('>');
				String replacement = stringBuilder.ToString();
#if DO_NOT_CHANGE || OUTPUT_TO_CONSOLE
				Console.Out.WriteLine("{0} --> {1}", match.Value, replacement);
#endif
#if DO_NOT_CHANGE
				return(match.Value);
#else
				changeCount++;
				return(replacement);
#endif
			});
			outChangeCount = changeCount;
			return(text);
		}

		private IEnumerable<String> alphabetiseAttributes(Group attributes, Group attributeNames) {
			return(
				attributes.Captures
					.Cast<Capture>()
					.Select((c, i) => new Tuple<String, String>(attributeNames.Captures[i].Value, c.Value))
					.OrderBy(t => t.Item1)
					.Select(t => t.Item2)
			);
		}

		/*
			NOTE: a regular expression is used to avoid issues associated with parsing and then outputting xml.
			This solution preserves the original formatting of the xml document with the ONLY change being alphabetisation.
			Comments, CDATA, and processing-instructions are accounted for.
		*/

		/*
			JUSTIFICATION: XML does not care about attribute order, the operation being performed here is ultimately just text manipulation.
		*/

		private static readonly Regex regex = new Regex(@"

			# element

			<
				(?'elementName'
					[\w\-\.]+\b
				)
				(?'attribute'
					\s*
					(?'attributeName'
						\b[\w\-\.]+\b
					)
					\s*
					=
					\s*
					(?:
						(?:')(?:[^']*)'
					|
						(?:"")(?:[^""]*)""
					)
				)*
				(?'elementTail'
					\s*
					\/?
				)
			>

		|

			# comment

			<!--
				(?:
					(?:(?!-->)[\s\S])*
				)
			-->

		|

			# literal

			<!\[CDATA\[
				(?:
					(?:(?!\]\]>)[\s\S])*
				)
			\]\]>

		|

			# processing-instruction

			<\?
				(?:
					[\w\-\.]+\b
				)
				(?:
					(?:(?!\?>)[\s\S])*
				)
			\?>

		", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

	}

}