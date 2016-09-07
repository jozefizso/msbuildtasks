#region Copyright © 2016 Jozef Izso. All rights reserved.
/*
Copyright © 2016 Jozef Izso. All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
   derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS" AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
*/
#endregion

using System;
using System.IO;
using System.Linq;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using YamlDotNet.RepresentationModel;


namespace MSBuild.Community.Tasks
{
    /// <summary>
    /// Reads a value from a YAML document using path expression.
    /// </summary>
    public class YamlRead : Task
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:YamlRead"/> class.
        /// </summary>
        public YamlRead()
        {
        }
        
        #region Properties
        private string _yamlFileName;

        /// <summary>
        /// Gets or sets the name of the YAML file.
        /// </summary>
        /// <value>The name of the YAML file.</value>
        [Required]
        public string YamlFileName
        {
            get { return _yamlFileName; }
            set { _yamlFileName = value; }
        }

        private string _path;

        /// <summary>
        /// Gets or sets the path expression.
        /// </summary>
        /// <value>The path expression.</value>
        [Required]
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        private string _value;

        /// <summary>
        /// Gets the value read from file.
        /// </summary>
        /// <value>The value.</value>
        /// <remarks>
        /// If the path expression returns multiple nodes, values will be semicolon delimited.
        /// </remarks>
        [Output]
        public string Value
        {
            get { return _value; }
        }
        #endregion

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            try
            {
                Log.LogMessage(Properties.Resources.YamlReadDocument, _yamlFileName);

                var input = File.OpenText(_yamlFileName);
                var yaml = new YamlStream();
                yaml.Load(input);

                var doc = yaml.Documents[0];
                var result = GetValue(doc, _path, out _value);

                if (true)
                {
                    Log.LogMessage(Properties.Resources.YamlReadResult, _value);
                }

                return result;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private static bool GetValue(YamlDocument document, string path, out string value)
        {
            value = null;

            var nodeNames = path.Split('.');
            var currentNode = document.RootNode;

            foreach (var name in nodeNames)
            {
                switch (currentNode.NodeType)
                {
                    case YamlNodeType.Mapping:
                        var mn = (YamlMappingNode)currentNode;

                        var kv = mn.Children.FirstOrDefault(n => n.Key.NodeType == YamlNodeType.Scalar && ((YamlScalarNode)n.Key).Value == name);
                        var subNode = kv.Value;

                        if (subNode != null)
                        {
                            currentNode = subNode;
                        }
                        else
                        {
                            return false;
                        }

                        break;
                    default:
                        return false;
                }
            }

            value = currentNode.ToString();
            return true;
        }

    }
}
