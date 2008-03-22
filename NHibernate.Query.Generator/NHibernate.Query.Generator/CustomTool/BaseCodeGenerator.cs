#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


namespace CustomToolGenerator {

	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Runtime.InteropServices;

	/// <summary>
	/// A managed wrapper for VS's concept of an IVsSingleFileGenerator which is
	/// a custom tool invoked during the build which can take any file as an input
	/// and provide a compilable code file as output.
	/// </summary>
	public abstract class BaseCodeGenerator : IVsSingleFileGenerator {

		private IVsGeneratorProgress	codeGeneratorProgress;
		private string					codeFileNameSpace	= String.Empty;
		private string					codeFilePath		= String.Empty;

		/// <summary>
		/// namespace for the file.
		/// </summary>
		protected string FileNameSpace {
			get {
				return codeFileNameSpace;
			}
		}

		/// <summary>
		/// file-path for the input file.
		/// </summary>
		protected string InputFilePath {
			get {
				return codeFilePath;
			}
		}

		/// <summary>
		/// interface to the VS shell object we use to tell our
		/// progress while we are generating.
		/// </summary>
		internal IVsGeneratorProgress CodeGeneratorProgress {
			get {
				return codeGeneratorProgress;
			}
		}

		/// <summary>
		/// gets the default extension for this generator
		/// </summary>
		/// <returns>string with the default extension for this generator</returns>
		public abstract string GetDefaultExtension();

		/// <summary>
		/// the method that does the actual work of generating code given the input
		/// file.
		/// </summary>
		/// <param name="inputFileName">input file name</param>
		/// <param name="inputFileContent">file contents as a string</param>
		/// <returns>the generated code file as a byte-array</returns>
		protected abstract byte[] GenerateCode(string inputFileName, string inputFileContent);

		/// <summary>
		/// method that will communicate an error via the shell callback mechanism.
		/// </summary>
		/// <param name="warning">true if this is a warning</param>
		/// <param name="level">level or severity</param>
		/// <param name="message">text displayed to the user</param>
		/// <param name="line">line number of error/warning</param>
		/// <param name="column">column number of error/warning</param>
		protected virtual void GeneratorErrorCallback(bool warning, int level, string message, int line, int column) {
			IVsGeneratorProgress progress = CodeGeneratorProgress;
			if (progress != null) {
				progress.GeneratorError(warning, level, message, line, column);
			}
		}

        /// <summary>
        /// main method that the VS shell calls to do the generation
        /// </summary>
        /// <param name="wszInputFilePath">path to the input file</param>
        /// <param name="bstrInputFileContents">contents of the input file as a string (shell handles UTF-8 to Unicode and those types of conversions)</param>
        /// <param name="wszDefaultNamespace">default namespace for the generated code file</param>
        /// <param name="rgbOutputFileContents">byte-array of output file contents</param>
        /// <param name="pcbOutput">count of bytes in the output byte-array</param>
        /// <param name="pGenerateProgress">interface to send progress updates to the shell</param>
		public void Generate(string wszInputFilePath, 
                             string bstrInputFileContents, 
                             string wszDefaultNamespace, 
			                 out IntPtr rgbOutputFileContents, 
                             out int pcbOutput, 
                             IVsGeneratorProgress pGenerateProgress) {

			if (bstrInputFileContents == null) {
				throw new ArgumentNullException(bstrInputFileContents);
			}

			codeFilePath = wszInputFilePath;
			codeFileNameSpace = wszDefaultNamespace;
			codeGeneratorProgress = pGenerateProgress;

			byte[] bytes = GenerateCode(wszInputFilePath, bstrInputFileContents);

			if (bytes == null) {
				rgbOutputFileContents = IntPtr.Zero;
				pcbOutput = 0;
			}
			else {
				pcbOutput = bytes.Length;
				rgbOutputFileContents = Marshal.AllocCoTaskMem(pcbOutput);
				Marshal.Copy(bytes, 0, rgbOutputFileContents, pcbOutput);                
			}
		}

        /// <summary>
        /// method to return a byte-array given a Stream
        /// </summary>
        /// <param name="stream">stream to convert to a byte-array</param>
        /// <returns>the stream's contents as a byte-array</returns>
        protected byte[] StreamToBytes(Stream stream) {

            if (stream.Length == 0) {
                return new byte[] { };
            }

            long position = stream.Position;
            stream.Position = 0;
            byte[] bytes = new byte[(int)stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Position = position;

            return bytes;
        }
    }
}