using Qonq.BlueSky.Model;
using System.Text.RegularExpressions;
using Qonq.BlueSky.Helper.Domain;
using Qonq.BlueSky.Helper.Strings;

namespace Qonq.BlueSky.Helper
{
    public static class Facets
    {
        public static List<Facet> DetectFacets(UnicodeString text)
        {
            List<Facet> facets = new List<Facet>();
            Match match;

            // Mentions
            Regex re = new Regex(@"(^|\s|\()(@)([a-zA-Z0-9.-]+)(\b)");
            foreach (Match m in re.Matches(text.Utf16))
            {
                if (!Test.IsValidDomain(m.Groups[3].Value) && !m.Groups[3].Value.EndsWith(".test"))
                {
                    continue; // probably not a handle
                }

                int start = text.Utf16.IndexOf(m.Groups[3].Value, m.Index) - 1;
                facets.Add(new Facet
                {
                    Type = "app.bsky.richtext.facet",
                    Index = new Index
                    {
                        ByteStart = text.Utf16IndexToUtf8Index(start),
                        ByteEnd = text.Utf16IndexToUtf8Index(start + m.Groups[3].Value.Length + 1)
                    },
                    Features = new List<Feature>
                {
                    new Feature
                    {
                        Type = "app.bsky.richtext.facet#mention",
                        Did = m.Groups[3].Value // must be resolved afterwards
                    }
                }
                });
            }

            // Links
            re = new Regex(@"(^|\s|\()((https?:\/\/[\S]+)|((?<domain>[a-z][a-z0-9]*(\.[a-z0-9]+)+)[\S]*))", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            foreach (Match m in re.Matches(text.Utf16))
            {
                string uri = m.Groups[2].Value;
                if (!uri.StartsWith("http"))
                {
                    string domain = m.Groups["domain"].Value;
                    if (string.IsNullOrEmpty(domain) || !Test.IsValidDomain(domain))
                    {
                        continue;
                    }
                    uri = $"https://{uri}";
                }
                int start = text.Utf16.IndexOf(m.Groups[2].Value, m.Index);
                var index = new { start, end = start + m.Groups[2].Value.Length };

                // Strip ending punctuation
                if (Regex.IsMatch(uri, @"[.,;!?]$"))
                {
                    uri = uri.Substring(0, uri.Length - 1);
                    index = new { start, end = index.end - 1 };
                }
                if (Regex.IsMatch(uri, @"[)]$") && !uri.Contains("("))
                {
                    uri = uri.Substring(0, uri.Length - 1);
                    index = new { start, end = index.end - 1 };
                }
                facets.Add(new Facet
                {
                    Type = "app.bsky.richtext.facet",
                    Index = new Index
                    {
                        ByteStart = text.Utf16IndexToUtf8Index(index.start),
                        ByteEnd = text.Utf16IndexToUtf8Index(index.end)
                    },
                    Features = new List<Feature>
                {
                    new Feature
                    {
                        Type = "app.bsky.richtext.facet#link",
                        Uri = uri
                    }
                }
                });
            }

            // Tags
            re = new Regex(@"(?:^|\s)(#[^\d\s]\S*)(?=\s)?");
            foreach (Match m in re.Matches(text.Utf16))
            {
                string tag = m.Value;
                bool hasLeadingSpace = Regex.IsMatch(tag, @"^\s");

                tag = tag.Trim().TrimEnd(new char[] { '.', ',', ';', '!', '?' }); // strip ending punctuation

                // Inclusive of #, max of 64 chars
                if (tag.Length > 66) continue;

                int index = m.Index + (hasLeadingSpace ? 1 : 0);

                facets.Add(new Facet
                {
                    Type = "app.bsky.richtext.facet",
                    Index = new Index
                    {
                        ByteStart = text.Utf16IndexToUtf8Index(index),
                        ByteEnd = text.Utf16IndexToUtf8Index(index + tag.Length) // inclusive of last char
                    },
                    Features = new List<Feature>
                {
                    new Feature
                    {
                        Type = "app.bsky.richtext.facet#tag",
                        Tag = tag.TrimStart('#')
                    }
                }
                });
            }

            return facets.Count > 0 ? facets : null;
        }
    }
}
