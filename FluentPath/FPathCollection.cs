// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IO = System.IO;
using System.Security.AccessControl;

namespace FluentPath {
    public class FPathCollection : IEnumerable<FPath> {
        private IEnumerable<string> _paths;
        private FPathCollection _previousPaths;

        /// <summary>
        /// Creates a collection of paths from a list of path strings.
        /// Avoid using directly, and use one of the methods on Path instead.
        /// </summary>
        /// <param name="paths">The list of path strings.</param>
        public FPathCollection(IEnumerable<string> paths) : this(paths, null) {
        }

        /// <summary>
        /// Creates a collection of paths from a list of path strings and a previous list of path strings.
        /// Avoid using directly, and use one of the methods on Path instead.
        /// </summary>
        /// <param name="paths">The list of path strings in the set.</param>
        /// <param name="previousPaths">The list of path strings in the previous set.</param>
        public FPathCollection(IEnumerable<string> paths, FPathCollection previousPaths) {
            _paths = paths;
            _previousPaths = previousPaths;
        }

        public IEnumerator<FPath> GetEnumerator() {
            return new FPathEnumerator(_paths);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return new FPathEnumerator(_paths);
        }

        /// <summary>
        /// Changes the path on each path in the set.
        /// Does not do any physical change to the file system.
        /// </summary>
        /// <param name="extensionTransformation">A function that maps each path to an extension.</param>
        /// <returns>The set</returns>
        public FPathCollection ChangeExtension(Func<FPath, string> extensionTransformation) {
            var result = new HashSet<string>();
            foreach (var path in _paths) {
                var p = new FPath(path);
                result.Add(
                    p.ChangeExtension(extensionTransformation(p)).ToString());
            }
            return new FPathCollection(result, this);
        }

        /// <summary>
        /// Changes the path on each path in the set.
        /// Does not do any physical change to the file system.
        /// </summary>
        /// <param name="newExtension">The new extension.</param>
        /// <returns>The set</returns>
        public FPathCollection ChangeExtension(string newExtension) {
            return ChangeExtension(p => newExtension);
        }

        /// <summary>
        /// Combines each path in the set with the specified file or directory name.
        /// Does not do any physical change to the file system.
        /// </summary>
        /// <param name="directoryNameGenerator">A function that maps each path to a file or directory name.</param>
        /// <returns>The set</returns>
        public FPathCollection Combine(Func<FPath, string> directoryNameGenerator) {
            var result = new HashSet<string>();
            foreach (var path in _paths) {
                var p = new FPath(path);
                if (p.IsDirectory) {
                    result.Add(
                        p.Combine(directoryNameGenerator(p)).ToString());
                }
            }
            return new FPathCollection(result, this);
        }

        /// <summary>
        /// Combines each path in the set with the specified file or directory name.
        /// Does not do any physical change to the file system.
        /// </summary>
        /// <param name="directoryName">A file or directory name.</param>
        /// <returns>The set</returns>
        public FPathCollection Combine(string directoryName) {
            return Combine(p => directoryName);
        }

        /// <summary>
        /// Does a copy of all files and directories in the set.
        /// </summary>
        /// <param name="pathMapping">
        /// A function that determines the destination path for each source path.
        /// If the function returns a null path, the file or directory is not copied.
        /// </param>
        /// <param name="overwrite">True if destination files should be overwritten. Default is false.</param>
        /// <param name="recursive">True if the copy should be deep and go into subdirectories recursively. Default is false.</param>
        /// <returns>The set</returns>
        public FPathCollection Copy(Func<FPath, FPath> pathMapping, bool overwrite = false, bool recursive = false) {
            var result = new HashSet<string>();
            foreach (var path in _paths) {
                var source = new FPath(path);
                var dest = pathMapping(source);
                if (dest != null) {
                    source.Copy(dest, overwrite, recursive);
                    result.Add(dest.ToString());
                }
            }
            return new FPathCollection(result, this);
        }

        /// <summary>
        /// Creates subdirectories for each directory.
        /// </summary>
        /// <param name="directoryNameGenerator">
        /// A function that returns the new directory name for each path.
        /// If the function returns null, no directory is created.
        /// </param>
        /// <returns>The set</returns>
        public FPathCollection CreateDirectory(Func<FPath, string> directoryNameGenerator) {
            var result = new HashSet<string>();
            foreach (var path in _paths) {
                var p = new FPath(path);
                var dest = directoryNameGenerator(p);
                if (dest != null) {
                    result.Add(
                        p.CreateSubDirectory(dest).ToString());
                }
            }
            return new FPathCollection(result, this);
        }

        /// <summary>
        /// Creates subdirectories for each directory.
        /// </summary>
        /// <param name="directoryNameGenerator">
        /// A function that returns the new directory name for each path.
        /// If the function returns null, no directory is created.
        /// </param>
        /// <returns>The set</returns>
        public FPathCollection CreateDirectory(Func<FPath, FPath> directoryNameGenerator) {
            var result = new HashSet<string>();
            foreach (var path in _paths) {
                var p = new FPath(path);
                var dest = directoryNameGenerator(p);
                if (dest != null) {
                    result.Add(
                        FPath.CreateDirectory(dest).ToString());
                }
            }
            return new FPathCollection(result, this);
        }

        /// <summary>
        /// Creates subdirectories for each directory.
        /// </summary>
        /// <param name="directoryName">The name of the new directory.</param>
        /// <returns>The set</returns>
        public FPathCollection CreateDirectory(string directoryName) {
            return CreateDirectory(p => directoryName);
        }

        /// <summary>
        /// Decrypts all files in the set.
        /// </summary>
        /// <returns>The set</returns>
        public FPathCollection Decrypt() {
            foreach (var path in _paths) {
                new FPath(path).Decrypt();
            }
            return this;
        }

        /// <summary>
        /// Deletes all files and folders in the set, including non-empty directories if recursive is true.
        /// </summary>
        /// <param name="recursive">If true, also deletes the contents of directories. Default is false.</param>
        /// <returns>The set of parent directories of all deleted file system entries.</returns>
        public FPathCollection Delete(bool recursive = false) {
            var result = new HashSet<string>();
            foreach (var path in _paths) {
                var p = new FPath(path);
                result.Add(p.Parent.ToString());
                p.Delete(recursive);
            }
            return new FPathCollection(result, this);
        }

        /// <summary>
        /// Encrypts all files in the set.
        /// </summary>
        /// <returns>The set</returns>
        public FPathCollection Encrypt() {
            foreach (var path in _paths) {
                new FPath(path).Encrypt();
            }
            return this;
        }

        /// <summary>
        /// Filters the set according to the predicate.
        /// </summary>
        /// <param name="predicate">A predicate that returns true for the entries that must be in the returned set.</param>
        /// <returns>The filtered set.</returns>
        public FPathCollection Where(Predicate<FPath> predicate) {
            var result = new HashSet<string>();
            foreach (var path in _paths) {
                if (predicate(new FPath(path))) {
                    result.Add(path);
                }
            }
            return new FPathCollection(result, this);
        }

        /// <summary>
        /// Executes an action for each file or folder in the set.
        /// </summary>
        /// <param name="action">An action that takes the path of each entry as its parameter.</param>
        /// <returns>The set</returns>
        public FPathCollection ForEach(Action<FPath> action) {
            foreach(var path in _paths) {
                action(new FPath(path));
            }
            return this;
        }

        /// <summary>
        /// Gets all the subdirectories of folders in the set that match the provided pattern and using the provided options.
        /// </summary>
        /// <param name="searchPattern">A search pattern such as "*.jpg". Default is "*".</param>
        /// <param name="recursive">True if subdirectories should also be searched recursively. Default is false.</param>
        /// <returns>The set of matching subdirectories.</returns>
        public FPathCollection Directories(string searchPattern = "*", bool recursive = false) {
            var result = new HashSet<string>();
            foreach (var path in _paths) {
                foreach (var dir in IO.Directory.GetDirectories(path, searchPattern, recursive ? IO.SearchOption.AllDirectories : IO.SearchOption.TopDirectoryOnly)) {
                    result.Add(dir);
                }
            }
            return new FPathCollection(result, this);
        }

        /// <summary>
        /// Gets all the files under the directories of the set that match the pattern, going recursively into subdirectories if recursive is set to true.
        /// </summary>
        /// <param name="searchPattern">A search pattern such as "*.jpg". Default is "*".</param>
        /// <param name="recursive">If true, subdirectories are explored as well. Default is false.</param>
        /// <returns>The set of files that match the pattern.</returns>
        public FPathCollection Files(string searchPattern = "*", bool recursive = false) {
            var result = new HashSet<string>();
            foreach (var path in _paths) {
                foreach (var dir in IO.Directory.GetFiles(path, searchPattern, recursive ? IO.SearchOption.AllDirectories : IO.SearchOption.TopDirectoryOnly)) {
                    result.Add(dir);
                }
            }
            return new FPathCollection(result, this);
        }

        /// <summary>
        /// Gets all the files and subdirectories under the directories of the set that match the pattern, going recursively into subdirectories if recursive is set to true.
        /// </summary>
        /// <param name="searchPattern">A search pattern such as "*.jpg". Default is "*".</param>
        /// <param name="recursive">If true, subdirectories are explored as well. Default is false.</param>
        /// <returns>The set of files and folders that match the pattern.</returns>
        public FPathCollection FileSystemEntries(string searchPattern = "*", bool recursive = false) {
            var result = new HashSet<string>();
            foreach (var path in _paths) {
                foreach (var dir in IO.Directory.GetFileSystemEntries(path, searchPattern, recursive ? IO.SearchOption.AllDirectories : IO.SearchOption.TopDirectoryOnly)) {
                    result.Add(dir);
                }
            }
            return new FPathCollection(result, this);
        }

        /// <summary>
        /// Maps all the paths in the set to a new set of paths using the provided mapping function.
        /// </summary>
        /// <param name="pathMapping">A function that takes a path and returns a transformed path.</param>
        /// <returns>The mapped set.</returns>
        public FPathCollection Map(Func<FPath, FPath> pathMapping) {
            var result = new HashSet<string>();
            foreach(var path in _paths) {
                result.Add(pathMapping(new FPath(path)).ToString());
            }
            return new FPathCollection(result, this);
        }

        /// <summary>
        /// Moves all the files and folders in the set to new locations as specified by the mapping function.
        /// </summary>
        /// <param name="pathMapping">The function that maps from the current path to the new one.</param>
        /// <returns>The moved set.</returns>
        public FPathCollection Move(Func<FPath, FPath> pathMapping) {
            var result = new HashSet<string>();
            foreach (var path in _paths) {
                var source = new FPath(path);
                var dest = pathMapping(source);
                source.Move(dest);
                result.Add(dest.ToString());
            }
            return new FPathCollection(result, this);
        }

        /// <summary>
        /// Opens all the files in the set and hands them to the provided action.
        /// </summary>
        /// <param name="action">The action to perform on the open files.</param>
        /// <param name="mode">The FileMode to use. Default is OpenOrCreate.</param>
        /// <param name="access">The FileAccess to use. Default is ReadWrite.</param>
        /// <param name="share">The FileShare to use. Default is None.</param>
        /// <returns>The set</returns>
        public FPathCollection Open(Action<IO.FileStream> action, IO.FileMode mode = IO.FileMode.OpenOrCreate, IO.FileAccess access = IO.FileAccess.ReadWrite, IO.FileShare share = IO.FileShare.None) {
            foreach (var path in _paths) {
                using (var stream = IO.File.Open(path, mode, access, share)) {
                    action(stream);
                }
            }
            return this;
        }

        /// <summary>
        /// Opens all the files in the set and hands them to the provided action.
        /// </summary>
        /// <param name="action">The action to perform on the open streams.</param>
        /// <param name="mode">The FileMode to use. Default is OpenOrCreate.</param>
        /// <param name="access">The FileAccess to use. Default is ReadWrite.</param>
        /// <param name="share">The FileShare to use. Default is None.</param>
        /// <returns>The set</returns>
        public FPathCollection Open(Action<IO.FileStream, FPath> action, IO.FileMode mode = IO.FileMode.OpenOrCreate, IO.FileAccess access = IO.FileAccess.ReadWrite, IO.FileShare share = IO.FileShare.None) {
            foreach (var path in _paths) {
                using (var stream = IO.File.Open(path, mode, access, share)) {
                    action(stream, new FPath(path));
                }
            }
            return this;
        }

        /// <summary>
        /// The previous set, from which the current one was created.
        /// </summary>
        /// <returns>The previous set.</returns>
        public FPathCollection Previous() {
            return _previousPaths;
        }


        /// <summary>
        /// Reads all text in files in the set and hands the results to the provided action.
        /// </summary>
        /// <param name="action">An action that takes the contents of the file.</param>
        /// <returns>The set</returns>
        public FPathCollection Read(Action<string> action) {
            foreach (var path in _paths) {
                action(IO.File.ReadAllText(path));
            }
            return this;
        }

        /// <summary>
        /// Reads all text in files in the set and hands the results to the provided action.
        /// </summary>
        /// <param name="action">An action that takes the contents of the file.</param>
        /// <param name="encoding">The encoding to use when reading the file.</param>
        /// <returns>The set</returns>
        public FPathCollection Read(Action<string> action, Encoding encoding) {
            foreach (var path in _paths) {
                action(IO.File.ReadAllText(path, encoding));
            }
            return this;
        }

        /// <summary>
        /// Reads all text in files in the set and hands the results to the provided action.
        /// </summary>
        /// <param name="action">An action that takes the contents of the file and its path.</param>
        /// <returns>The set</returns>
        public FPathCollection Read(Action<string, FPath> action) {
            foreach (var path in _paths) {
                action(IO.File.ReadAllText(path), new FPath(path));
            }
            return this;
        }

        /// <summary>
        /// Reads all text in files in the set and hands the results to the provided action.
        /// </summary>
        /// <param name="action">An action that takes the contents of the file and its path.</param>
        /// <param name="encoding">The encoding to use when reading the file.</param>
        /// <returns>The set</returns>
        public FPathCollection Read(Action<string, FPath> action, Encoding encoding) {
            foreach (var path in _paths) {
                action(IO.File.ReadAllText(path, encoding), new FPath(path));
            }
            return this;
        }

        /// <summary>
        /// Reads all the bytes in a file and hands them to the provided action.
        /// </summary>
        /// <param name="action">An action that takes an array of bytes.</param>
        /// <returns>The set</returns>
        public FPathCollection ReadBytes(Action<byte[]> action) {
            foreach (var path in _paths) {
                action(IO.File.ReadAllBytes(path));
            }
            return this;
        }

        /// <summary>
        /// Reads all the bytes in a file and hands them to the provided action.
        /// </summary>
        /// <param name="action">An action that takes an array of bytes and a path.</param>
        /// <returns>The set</returns>
        public FPathCollection ReadBytes(Action<byte[], FPath> action) {
            foreach (var path in _paths) {
                action(IO.File.ReadAllBytes(path), new FPath(path));
            }
            return this;
        }

        /// <summary>
        /// Sets the access control security on all files and directories in the set.
        /// </summary>
        /// <param name="security">The security to apply.</param>
        /// <returns>The set</returns>
        public FPathCollection AccessControl(FileSystemSecurity security) {
            return AccessControl(p => security);
        }

        /// <summary>
        /// Sets the access control security on all files and directories in the set.
        /// </summary>
        /// <param name="securityFunction">A function that returns the security for each path.</param>
        /// <returns>The set</returns>
        public FPathCollection AccessControl(Func<FPath, FileSystemSecurity> securityFunction) {
            foreach (var path in _paths) {
                var p = new FPath(path);
                p.SetAccessControl(securityFunction(p));
            }
            return this;
        }

        /// <summary>
        /// Sets attributes on all files in the set.
        /// </summary>
        /// <param name="attributes">The attributes to set.</param>
        /// <returns>The set</returns>
        public FPathCollection Attributes(IO.FileAttributes attributes) {
            return Attributes(p => attributes);
        }

        /// <summary>
        /// Sets attributes on all files in the set.
        /// </summary>
        /// <param name="attributeFunction">A function that gives the attributes to set for each path.</param>
        /// <returns>The set</returns>
        public FPathCollection Attributes(Func<FPath, IO.FileAttributes> attributeFunction) {
            foreach (var path in _paths) {
                var p = new FPath(path);
                p.SetAttributes(attributeFunction(p));
            }
            return this;
        }

        /// <summary>
        /// Sets the creation time across the set.
        /// </summary>
        /// <param name="creationTime">The time to set.</param>
        /// <returns>The set</returns>
        public FPathCollection CreationTime(DateTime creationTime) {
            return CreationTime(p => creationTime);
        }

        /// <summary>
        /// Sets the creation time across the set.
        /// </summary>
        /// <param name="creationTimeFunction">A function that returns the new creation time for each path.</param>
        /// <returns>The set</returns>
        public FPathCollection CreationTime(Func<FPath, DateTime> creationTimeFunction) {
            foreach (var path in _paths) {
                var p = new FPath(path);
                p.SetCreationTime(creationTimeFunction(p));
            }
            return this;
        }

        /// <summary>
        /// Sets the UTC creation time across the set.
        /// </summary>
        /// <param name="creationTime">The time to set.</param>
        /// <returns>The set</returns>
        public FPathCollection CreationTimeUtc(DateTime creationTimeUtc) {
            return CreationTimeUtc(p => creationTimeUtc);
        }

        /// <summary>
        /// Sets the UTC creation time across the set.
        /// </summary>
        /// <param name="creationTimeFunction">A function that returns the new time for each path.</param>
        /// <returns>The set</returns>
        public FPathCollection CreationTimeUtc(Func<FPath, DateTime> creationTimeFunctionUtc) {
            foreach (var path in _paths) {
                var p = new FPath(path);
                p.SetCreationTimeUtc(creationTimeFunctionUtc(p));
            }
            return this;
        }

        /// <summary>
        /// Sets the last access time across the set.
        /// </summary>
        /// <param name="creationTime">The time to set.</param>
        /// <returns>The set</returns>
        public FPathCollection LastAccessTime(DateTime lastAccessTime) {
            return LastAccessTime(p => lastAccessTime);
        }

        /// <summary>
        /// Sets the last access time across the set.
        /// </summary>
        /// <param name="creationTimeFunction">A function that returns the new time for each path.</param>
        /// <returns>The set</returns>
        public FPathCollection LastAccessTime(Func<FPath, DateTime> lastAccessTimeFunction) {
            foreach (var path in _paths) {
                var p = new FPath(path);
                p.SetLastAccessTime(lastAccessTimeFunction(p));
            }
            return this;
        }

        /// <summary>
        /// Sets the UTC last access time across the set.
        /// </summary>
        /// <param name="creationTime">The time to set.</param>
        /// <returns>The set</returns>
        public FPathCollection LastAccessTimeUtc(DateTime lastAccessTimeUtc) {
            return LastAccessTimeUtc(p => lastAccessTimeUtc);
        }

        /// <summary>
        /// Sets the UTC last access time across the set.
        /// </summary>
        /// <param name="creationTimeFunction">A function that returns the new time for each path.</param>
        /// <returns>The set</returns>
        public FPathCollection LastAccessTimeUtc(Func<FPath, DateTime> lastAccessTimeFunctionUtc) {
            foreach (var path in _paths) {
                var p = new FPath(path);
                p.SetLastAccessTimeUtc(lastAccessTimeFunctionUtc(p));
            }
            return this;
        }

        /// <summary>
        /// Sets the last write time across the set.
        /// </summary>
        /// <param name="creationTime">The time to set.</param>
        /// <returns>The set</returns>
        public FPathCollection LastWriteTime(DateTime lastWriteTime) {
            return LastWriteTime(p => lastWriteTime);
        }

         /// <summary>
        /// Sets the last write time across the set.
        /// </summary>
        /// <param name="creationTimeFunction">A function that returns the new time for each path.</param>
        /// <returns>The set</returns>
        public FPathCollection LastWriteTime(Func<FPath, DateTime> lastWriteTimeFunction) {
            foreach (var path in _paths) {
                var p = new FPath(path);
                p.SetLastWriteTime(lastWriteTimeFunction(p));
            }
            return this;
        }

        /// <summary>
        /// Sets the UTC last write time across the set.
        /// </summary>
        /// <param name="creationTime">The time to set.</param>
        /// <returns>The set</returns>
        public FPathCollection LastWriteTimeUtc(DateTime lastWriteTimeUtc) {
            return LastWriteTimeUtc(p => lastWriteTimeUtc);
        }

         /// <summary>
        /// Sets the UTC last write time across the set.
        /// </summary>
        /// <param name="creationTimeFunction">A function that returns the new time for each path.</param>
        /// <returns>The set</returns>
       public FPathCollection LastWriteTimeUtc(Func<FPath, DateTime> lastWriteTimeFunctionUtc) {
            foreach (var path in _paths) {
                var p = new FPath(path);
                p.SetLastWriteTimeUtc(lastWriteTimeFunctionUtc(p));
            }
            return this;
        }

        /// <summary>
        /// Goes up one level on each path in the set.
        /// </summary>
        /// <returns>The new set</returns>
        public FPathCollection Up() {
            return Up(1);
        }

        /// <summary>
        /// Goes up the specified number of levels on each path in the set.
        /// Never goes above the root of the drive.
        /// </summary>
        /// <param name="levels">The number of levels to go up.</param>
        /// <returns>The new set</returns>
        public FPathCollection Up(int levels) {
            var result = new HashSet<string>();
            foreach (var path in _paths) {
                result.Add(new FPath(path).Up(levels).ToString());
            }
            return new FPathCollection(result, this);
        }

        /// <summary>
        /// Writes to all files in the set using UTF8.
        /// </summary>
        /// <param name="text">The text to write.</param>
        /// <param name="append">True if the text should be appended to the existing contents. Default is false.</param>
        /// <returns>The set</returns>
        public FPathCollection Write(string text, bool append = false) {
            return Write(text, Encoding.UTF8, append);
        }

        /// <summary>
        /// Writes to all files in the set.
        /// </summary>
        /// <param name="text">The text to write.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="append">True if the text should be appended to the existing contents. Default is false.</param>
        /// <returns>The set</returns>
        public FPathCollection Write(string text, Encoding encoding, bool append = false) {
            return Write(p => text, encoding, append);
        }

        /// <summary>
        /// Writes to all files in the set.
        /// </summary>
        /// <param name="textFunction">A function that returns the text to write for each path.</param>
        /// <param name="append">True if the text should be appended to the existing contents. Default is false.</param>
        /// <returns>The set</returns>
        public FPathCollection Write(Func<FPath, string> textFunction, bool append = false) {
            return Write(textFunction, Encoding.UTF8, append);
        }

        /// <summary>
        /// Writes to all files in the set.
        /// </summary>
        /// <param name="textFunction">A function that returns the text to write for each path.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="append">True if the text should be appended to the existing contents. Default is false.</param>
        /// <returns>The set</returns>
        public FPathCollection Write(Func<FPath, string> textFunction, Encoding encoding, bool append = false) {
            foreach (var path in _paths) {
                var p = new FPath(path);
                p.Write(textFunction(p), encoding, append);
            }
            return this;
        }

        /// <summary>
        /// Writes to all files in the set.
        /// </summary>
        /// <param name="bytes">The byte array to write.</param>
        /// <returns>The set</returns>
        public FPathCollection WriteBytes(byte[] bytes) {
            return WriteBytes(p => bytes);
        }

        /// <summary>
        /// Writes to all files in the set.
        /// </summary>
        /// <param name="byteFunction">A function that returns a byte array to write for each path.</param>
        /// <returns>The set</returns>
        public FPathCollection WriteBytes(Func<FPath, byte[]> byteFunction) {
            foreach (var path in _paths) {
                var p = new FPath(path);
                p.WriteBytes(byteFunction(p));
            }
            return this;
        }
    }
}
