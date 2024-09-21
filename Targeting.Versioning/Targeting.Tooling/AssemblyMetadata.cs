// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Text.RegularExpressions;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;

namespace nanoFramework.Targeting.Tooling
{
    /// <summary>
    /// Some information about a nanoFramework assembly.
    /// </summary>
    public sealed class AssemblyMetadata
    {
        #region Construction
        /// <summary>
        /// Get the information about the nanoFramework assembly.
        /// </summary>
        /// <param name="assemblyFilePath">The path to the assembly. Can be the path to a *.pe, *.dll or *.exe file.
        /// if it is the path to a *.pe file, the *.dll/*.exe file should reside in the same directory.</param>
        public AssemblyMetadata(string assemblyFilePath)
        {
            AssemblyFilePath = assemblyFilePath;
            NanoFrameworkAssemblyFilePath = assemblyFilePath;
            if (Path.GetExtension(assemblyFilePath).ToLower() == ".pe")
            {
                string tryPath = Path.ChangeExtension(assemblyFilePath, ".dll");
                if (File.Exists(tryPath))
                {
                    AssemblyFilePath = tryPath;
                }
                else
                {
                    tryPath = Path.ChangeExtension(assemblyFilePath, ".exe");
                    if (File.Exists(tryPath))
                    {
                        AssemblyFilePath = tryPath;
                    }
                }
            }
            else
            {
                NanoFrameworkAssemblyFilePath = Path.ChangeExtension(assemblyFilePath, ".pe");
            }

            uint? nativeChecksum = null;
            if (File.Exists(NanoFrameworkAssemblyFilePath))
            {
                using (FileStream fs = File.Open(NanoFrameworkAssemblyFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    // read the PE checksum from the byte array at position 0x14
                    byte[] buffer = new byte[4];
                    fs.Position = 0x14;
                    fs.Read(buffer, 0, 4);
                    uint nativeMethodsChecksum = BitConverter.ToUInt32(buffer, 0);

                    if (nativeMethodsChecksum != 0)
                    {
                        // PEs with native methods checksum equal to 0 DO NOT require native support 
                        // OK to move to the next one
                        nativeChecksum = nativeMethodsChecksum;
                    }
                }
            }

            if (File.Exists(AssemblyFilePath))
            {
                var deCompiler = new CSharpDecompiler(AssemblyFilePath, new DecompilerSettings
                {
                    LoadInMemory = false,
                    ThrowOnAssemblyResolveErrors = false
                });
                string assemblyProperties = deCompiler.DecompileModuleAndAssemblyAttributesToString();

                // AssemblyVersion
                string pattern = @"(?<=AssemblyVersion\("")(.*)(?=\""\)])";
                MatchCollection match = Regex.Matches(assemblyProperties, pattern, RegexOptions.IgnoreCase);
                Version = match[0].Value;

                // AssemblyNativeVersion
                pattern = @"(?<=AssemblyNativeVersion\("")(.*)(?=\""\)])";
                match = Regex.Matches(assemblyProperties, pattern, RegexOptions.IgnoreCase);

                // only class libs have this attribute, therefore sanity check is required
                if (match.Count == 1)
                {
                    if (nativeChecksum is not null)
                    {
                        NativeAssembly = new NativeAssemblyMetadata(Path.GetFileNameWithoutExtension(AssemblyFilePath), match[0].Value, nativeChecksum.Value);
                    }
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Path to the EXE or DLL file.
        /// </summary>
        public string AssemblyFilePath
        {
            get;
        }

        /// <summary>
        /// Path to the PE file.
        /// </summary>
        public string NanoFrameworkAssemblyFilePath
        {
            get;
        }

        /// <summary>
        /// Assembly version of the EXE or DLL. Is <c>null</c> if the assembly does not exist.
        /// </summary>
        public string? Version
        {
            get;
        }

        /// <summary>
        /// Native assembly/implementation that should be part of the firmware/CLR instance fot
        /// this .NET assembly to function properly.
        /// Is <c>null</c> if the .NET assembly does not require a native implementation.
        /// </summary>
        public NativeAssemblyMetadata? NativeAssembly
        {
            get;
        }
        #endregion
    }
}
