// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.AccessControl;
using System.Text;
using IO = System.IO;

namespace FluentPath {

    [TypeConverter(typeof(FPathConverter))]
    public class FPath {
        private string _path;
        private FPath _previousPath;

        // Constructors

        /// <summary>
        /// Creates a new path from a string.
        /// You may also create a new path object from the Get method.
        /// </summary>
        /// <param name="path">The string representation of the path.</param>
        public FPath(string path) : this(path, null) {
        }

        /// <summary>
        /// Creates a new path from a string.
        /// You may also create a new path object from the Get method.
        /// </summary>
        /// <param name="path">The string representation of the path.</param>
        /// <param name="previousPath">The previous path.</param>
        public FPath(string path, FPath previousPath) {
            if (path == null) throw new InvalidOperationException("Path can't be null.");
            _path = path;
            _previousPath = previousPath;
        }

        // Conversion operators

        public static implicit operator string(FPath path) {
            return path.ToString();
        }

        public static implicit operator FPath(string path) {
            return new FPath(path);
        }

        // Path properties

        /// <summary>
        /// The name of the directory for that path.
        /// This is the string representation of the parent directory path.
        /// </summary>
        public string DirectoryName {
            get { return IO.Path.GetDirectoryName(_path); }
        }

        /// <summary>
        /// The extension for this path, including the ".".
        /// </summary>
        public string Extension {
            get { return IO.Path.GetExtension(_path); }
        }

        /// <summary>
        /// The filename or folder name for this path, including the extension.
        /// </summary>
        public string FileName {
            get { return IO.Path.GetFileName(_path); }
        }

        /// <summary>
        /// The filename or folder name for this path, without the extension.
        /// </summary>
        public string FileNameWithoutExtension {
            get { return IO.Path.GetFileNameWithoutExtension(_path); }
        }

        /// <summary>
        /// The fully qualified path string for this path.
        /// </summary>
        public string FullPath {
            get { return IO.Path.GetFullPath(_path); }
        }

        /// <summary>
        /// True if the path has an extension.
        /// </summary>
        public bool HasExtension {
            get { return IO.Path.HasExtension(_path); }
        }

        /// <summary>
        /// True if the path is fully-qualified.
        /// </summary>
        public bool IsRooted {
            get { return IO.Path.IsPathRooted(_path); }
        }

        /// <summary>
        /// The parent path for this path.
        /// </summary>
        public FPath Parent {
            get {
                var upStr = IO.Path.GetDirectoryName(this.ToString());
                if (upStr == null) return this;
                return new FPath(upStr, this);
            }
        }

        /// <summary>
        /// The previous path that was used to create this one.
        /// For example, Path.Get("\foo\bar").Parent.Previous
        /// will return the "\foo\bar" path object.
        /// </summary>
        public FPath Previous {
            get { return _previousPath; }
        }

        /// <summary>
        /// The root directory of this path, such as "C:\".
        /// </summary>
        public string PathRoot {
            get { return IO.Path.GetPathRoot(_path); }
        }

        // Directory & file properties

        /// <summary>
        /// The access control security information for the path.
        /// </summary>
        public FileSystemSecurity AccessControl {
            get {
                return IsDirectory ?
                    (FileSystemSecurity)IO.Directory.GetAccessControl(_path) :
                    (FileSystemSecurity)IO.File.GetAccessControl(_path);
            }
            set {
                if (IsDirectory) {
                    IO.Directory.SetAccessControl(_path, (DirectorySecurity)value);
                }
                else {
                    IO.File.SetAccessControl(_path, (FileSecurity)value);
                }
            }
        }

        /// <summary>
        /// The attributes for the file.
        /// </summary>
        public IO.FileAttributes Attributes {
            get {
                return IO.File.GetAttributes(_path);
            }
            set {
                IO.File.SetAttributes(_path, value);
            }
        }

        /// <summary>
        /// The creation time of this path.
        /// </summary>
        public DateTime CreationTime {
            get {
                return IsDirectory ?
                    IO.Directory.GetCreationTime(_path) :
                    IO.File.GetCreationTime(_path);
            }
            set {
                if (IsDirectory) {
                    IO.Directory.SetCreationTime(_path, value);
                }
                else {
                    IO.File.SetCreationTime(_path, value);
                }
            }
        }

        /// <summary>
        /// The UTC creation time of this path.
        /// </summary>
        public DateTime CreationTimeUtc {
            get {
                return IsDirectory ?
                    IO.Directory.GetCreationTimeUtc(_path) :
                    IO.File.GetCreationTimeUtc(_path);
            }
            set {
                if (IsDirectory) {
                    IO.Directory.SetCreationTimeUtc(_path, value);
                }
                else {
                    IO.File.SetCreationTimeUtc(_path, value);
                }
            }
        }

        /// <summary>
        /// The subdirectories under this path.
        /// </summary>
        public FPathCollection Directories {
            get { return GetDirectories(); }
        }

        /// <summary>
        /// True if the path exists in the file system.
        /// </summary>
        public bool Exists {
            get {
                if (IsDirectory) {
                    return IO.Directory.Exists(_path);
                }
                return IO.File.Exists(_path);
            }
        }

        /// <summary>
        /// The files under this path.
        /// </summary>
        public FPathCollection Files {
            get { return GetFiles(_path); }
        }

        /// <summary>
        /// The last access time of this path.
        /// </summary>
        public DateTime LastAccessTime {
            get {
                return IsDirectory ?
                    IO.Directory.GetLastAccessTime(_path) :
                    IO.File.GetLastAccessTime(_path);
            }
            set {
                if (IsDirectory) {
                    IO.Directory.SetLastAccessTime(_path, value);
                }
                else {
                    IO.File.SetLastAccessTime(_path, value);
                }
            }
        }

        /// <summary>
        /// The UTC last access time of this path.
        /// </summary>
        public DateTime LastAccessTimeUtc {
            get {
                return IsDirectory ?
                    IO.Directory.GetLastAccessTimeUtc(_path) :
                    IO.File.GetLastAccessTimeUtc(_path);
            }
            set {
                if (IsDirectory) {
                    IO.Directory.SetLastAccessTimeUtc(_path, value);
                }
                else {
                    IO.File.SetLastAccessTimeUtc(_path, value);
                }
            }
        }

        /// <summary>
        /// The last write time of this path.
        /// </summary>
        public DateTime LastWriteTime {
            get {
                return IsDirectory ?
                    IO.Directory.GetLastWriteTime(_path) :
                    IO.File.GetLastWriteTime(_path);
            }
            set {
                if (IsDirectory) {
                    IO.Directory.SetLastWriteTime(_path, value);
                }
                else {
                    IO.File.SetLastWriteTime(_path, value);
                }
            }
        }

        /// <summary>
        /// The UTC last write time of this path.
        /// </summary>
        public DateTime LastWriteTimeUtc {
            get {
                return IsDirectory ?
                    IO.Directory.GetLastWriteTimeUtc(_path) :
                    IO.File.GetLastWriteTimeUtc(_path);
            }
            set {
                if (IsDirectory) {
                    IO.Directory.SetLastWriteTimeUtc(_path, value);
                }
                else {
                    IO.File.SetLastWriteTimeUtc(_path, value);
                }
            }
        }

        /// <summary>
        /// True if this is the path of a directory in the file system.
        /// </summary>
        public bool IsDirectory {
            get { return IO.Directory.Exists(_path); }
        }

        // Overrides
        public override bool Equals(object obj) {
            var path = obj as FPath;
            if (path != null) return path.ToString() == ToString();
            var str = obj as string;
            if (str != null) return str == ToString();
            return false;
        }
		public static bool operator !=(FPath p1, FPath p2)
		{
			if (object.ReferenceEquals(p1, null)) return !object.ReferenceEquals(p2, null);
			return !p1.Equals(p2);
		}
		public static bool operator ==(FPath p1, FPath p2)
		{
			if (object.ReferenceEquals(p1, null)) return object.ReferenceEquals(p2, null);
			return p1.Equals(p2);
		}

        public override int GetHashCode() {
            return _path.GetHashCode();
        }

        public override string ToString() {
            return _path;
        }

        // Statics

        /// <summary>
        /// Creates a directory in the file system.
        /// </summary>
        /// <param name="directory">The path of the directory to create.</param>
        /// <returns>The path of the new directory.</returns>
        public static FPath CreateDirectory(FPath directory) {
            return directory.CreateDirectory();
        }

        /// <summary>
        /// Creates a directory in the file system.
        /// </summary>
        /// <param name="directory">The path of the directory to create.</param>
        /// <param name="directorySecurity">The security to apply to the new directory.</param>
        /// <returns>The path of the new directory.</returns>
        public static FPath CreateDirectory(FPath directory, DirectorySecurity directorySecurity) {
            return directory.CreateDirectory(directorySecurity);
        }

        /// <summary>
        /// Creates a directory in the file system.
        /// </summary>
        /// <param name="directoryName">The name of the directory to create.</param>
        /// <returns>The path of the new directory.</returns>
        public static FPath CreateDirectory(string directoryName) {
            return new FPath(directoryName).CreateDirectory();
        }

        /// <summary>
        /// Creates a directory in the file system.
        /// </summary>
        /// <param name="directoryName">The name of the directory to create.</param>
        /// <param name="directorySecurity">The security to apply to the new directory.</param>
        /// <returns>The path of the new directory.</returns>
        public static FPath CreateDirectory(string directoryName, DirectorySecurity directorySecurity) {
            return new FPath(directoryName).CreateDirectory(directorySecurity);
        }

        /// <summary>
        /// The current path for the application.
        /// </summary>
        public static FPath Current {
            get {
                return new FPath(IO.Directory.GetCurrentDirectory());
            }
            set {
                IO.Directory.SetCurrentDirectory(value.ToString());
            }
        }

        /// <summary>
        /// Creates a new path from its string representation.
        /// </summary>
        /// <param name="path">The string for the path.</param>
        /// <returns>The path object.</returns>
        public static FPath Get(string path) {
            return new FPath(path);
        }

        // Path API

        /// <summary>
        /// Changes the extension for this path.
        /// </summary>
        /// <param name="newExtension">The new extension.</param>
        /// <returns></returns>
        public FPath ChangeExtension(string newExtension) {
            return new FPath(IO.Path.ChangeExtension(ToString(), newExtension), this);
        }

        /// <summary>
        /// Combines the current path with another.
        /// </summary>
        /// <param name="path">The path to combine to the current one.</param>
        /// <returns>The combined path.</returns>
        public FPath Combine(string path) {
            return new FPath(IO.Path.Combine(ToString(), path), this);
        }

        /// <summary>
        /// Goes up the specified number of levels.
        /// Never goes higher than the root.
        /// </summary>
        /// <param name="levels">The number of levels.</param>
        /// <returns>The new path.</returns>
        public FPath Up(int levels) {
            var str = ToString();
            for (var i = 0; i < levels; i++) {
                var strUp = IO.Path.GetDirectoryName(str);
                if (strUp == null) {
                    strUp = str;
                    break;
                }
                str = strUp;
            }
            return new FPath(str, this);
        }

        // Directory & file API

        /// <summary>
        /// Creates the current path as a new directory in the file system.
        /// </summary>
        /// <returns>The path.</returns>
        public FPath CreateDirectory() {
            IO.Directory.CreateDirectory(_path);
            return this;
        }

        /// <summary>
        /// Creates the current path as a new directory in the file system.
        /// </summary>
        /// <param name="directorySecurity">The security to apply to the new directory.</param>
        /// <returns>The path.</returns>
        public FPath CreateDirectory(DirectorySecurity directorySecurity) {
            IO.Directory.CreateDirectory(_path, directorySecurity);
            return this;
        }

        /// <summary>
        /// Creates a subdirectory to the current path in the file system.
        /// </summary>
        /// <param name="directoryName">The name of the directory to create.</param>
        /// <returns>The path of the new directory.</returns>
        public FPath CreateSubDirectory(string directoryName) {
            return Combine(directoryName).CreateDirectory();
        }

        /// <summary>
        /// Creates a subdirectory to the current path in the file system.
        /// </summary>
        /// <param name="directoryName">The name of the directory to create.</param>
        /// <param name="directorySecurity">The security to apply to the new directory.</param>
        /// <returns>The path of the new directory.</returns>
        public FPath CreateSubDirectory(string directoryName, DirectorySecurity directorySecurity) {
            return Combine(directoryName).CreateDirectory(directorySecurity);
        }

        /// <summary>
        /// Copies the file or folder for this path to another location.
        /// </summary>
        /// <param name="destination">The destination path.</param>
        /// <param name="overwrite">True if the destination can be overwritten. Default is false.</param>
        /// <param name="recursive">True if the copy should be deep and include subdirectories recursively. Default is false.</param>
        /// <returns>The source path.</returns>
        public FPath Copy(FPath destination, bool overwrite = false, bool recursive = false) {
            return Copy(destination.ToString(), overwrite, recursive);
        }

        /// <summary>
        /// Copies the file or folder for this path to another location.
        /// </summary>
        /// <param name="destination">The destination path string.</param>
        /// <param name="overwrite">True if the destination can be overwritten. Default is false.</param>
        /// <param name="recursive">True if the copy should be deep and include subdirectories recursively. Default is false.</param>
        /// <returns>The source path.</returns>
        public FPath Copy(string destination, bool overwrite, bool recursive) {
            if (IsDirectory) {
                CopyDirectory(_path, destination, overwrite, recursive);
            }
            else {
                IO.File.Copy(_path, IO.Path.GetFileName(_path), overwrite);
            }
            return this;
        }

        private static void CopyDirectory(string source, string destination, bool overwrite, bool recursive) {
            if (recursive) {
                foreach (var subdirectory in IO.Directory.GetDirectories(source)) {
                    CopyDirectory(subdirectory,
                        IO.Path.Combine(destination, IO.Path.GetFileName(subdirectory)),
                        true, overwrite);
                }
            }
            foreach (var file in IO.Directory.GetFiles(source)) {
                IO.File.Copy(file, IO.Path.GetFileName(file), overwrite);
            }
        }

        /// <summary>
        /// Deletes this path from the file system.
        /// </summary>
        /// <returns>The path.</returns>
        public FPath Delete() {
            if (!IsDirectory) {
                IO.File.Delete(_path);
                return this;
            }
            return Delete(false);
        }

        /// <summary>
        /// Deletes this directory from the file system.
        /// </summary>
        /// <param name="recursive">True if subdirectories should be recursively deleted.</param>
        /// <returns>The path.</returns>
        public FPath Delete(bool recursive) {
            IO.Directory.Delete(_path, recursive);
            return this;
        }

        /// <summary>
        /// Gets all subdirectories of this path.
        /// </summary>
        /// <param name="searchPattern">The search pattern to use. Default is "*".</param>
        /// <param name="recursive">True if subdirectories should be recursively included.</param>
        /// <returns>The set of subdirectory paths.</returns>
        public FPathCollection GetDirectories(string searchPattern = "*", bool recursive = false) {
            return new FPathCollection(
                IO.Directory.GetDirectories(
                    _path, searchPattern,
                    recursive ? IO.SearchOption.AllDirectories : IO.SearchOption.TopDirectoryOnly),
                new FPathCollection(new string[] {_path}));
        }

        // TODO: implement full IQueryable instead.

        /// <summary>
        /// Creates a set from all the subdirectories that satisfy the specified predicate.
        /// </summary>
        /// <param name="predicate">A function that returns true if the directory should be included.</param>
        /// <param name="recursive">True if subdirectories should be recursively included.</param>
        /// <returns>The set of directories that satisfy the predicate.</returns>
        public FPathCollection GetDirectories(Predicate<FPath> predicate, bool recursive = false) {
            var result = new HashSet<string>();
            foreach (var dir in IO.Directory.GetDirectories(_path, "*", recursive ? IO.SearchOption.AllDirectories : IO.SearchOption.TopDirectoryOnly)) {
                if (predicate(new FPath(dir))) {
                    result.Add(dir);
                }
            }
            return new FPathCollection(result, new FPathCollection(new string[] { _path }));
        }

        /// <summary>
        /// Gets all files under this path.
        /// </summary>
        /// <param name="searchPattern">The search pattern to use. Default is "*".</param>
        /// <param name="recursive">True if subdirectories should be recursively included.</param>
        /// <returns>The set of file paths.</returns>
        public FPathCollection GetFiles(string searchPattern = "*", bool recursive = false) {
            return new FPathCollection(
                IO.Directory.GetFiles(
                    _path, searchPattern,
                    recursive ? IO.SearchOption.AllDirectories : IO.SearchOption.TopDirectoryOnly),
                new FPathCollection(new string[] {_path}));
        }

        // TODO: implement full IQueryable instead.

        /// <summary>
        /// Creates a set from all the files under the path that satisfy the specified predicate.
        /// </summary>
        /// <param name="predicate">A function that returns true if the path should be included.</param>
        /// <param name="recursive">True if subdirectories should be recursively included.</param>
        /// <returns>The set of paths that satisfy the predicate.</returns>
        public FPathCollection GetFiles(Predicate<FPath> predicate, bool recursive = false) {
            var result = new HashSet<string>();
            foreach (var file in IO.Directory.GetFiles(_path, "*", recursive ? IO.SearchOption.AllDirectories : IO.SearchOption.TopDirectoryOnly)) {
                if (predicate(new FPath(file))) {
                    result.Add(file);
                }
            }
            return new FPathCollection(result, new FPathCollection(new string[] { _path }));
        }

        /// <summary>
        /// Gets all files and subdirectories under this path.
        /// </summary>
        /// <param name="searchPattern">The search pattern to use. Default is "*".</param>
        /// <param name="recursive">True if subdirectories should be recursively included.</param>
        /// <returns>The set of file and subdirectory paths.</returns>
        public FPathCollection GetFileSystemEntries(string searchPattern = "*", bool recursive = false) {
            return new FPathCollection(
                IO.Directory.GetFileSystemEntries(
                    _path, searchPattern,
                    recursive ? IO.SearchOption.AllDirectories : IO.SearchOption.TopDirectoryOnly),
                new FPathCollection(new string[] {_path}));
        }

        // TODO: implement full IQueryable instead.

        /// <summary>
        /// Creates a set from all the files and subdirectories under the path that satisfy the specified predicate.
        /// </summary>
        /// <param name="predicate">A function that returns true if the path should be included.</param>
        /// <param name="recursive">True if subdirectories should be recursively included.</param>
        /// <returns>The set of fils and subdirectories that satisfy the predicate.</returns>
        public FPathCollection GetFileSystemEntries(Predicate<FPath> predicate, bool recursive = false) {
            var result = new HashSet<string>();
            foreach (var entry in IO.Directory.GetFileSystemEntries(_path, "*", recursive ? IO.SearchOption.AllDirectories : IO.SearchOption.TopDirectoryOnly)) {
                if (predicate(new FPath(entry))) {
                    result.Add(entry);
                }
            }
            return new FPathCollection(result, new FPathCollection(new string[] { _path }));
        }

        /// <summary>
        /// Makes this path the current path for the application.
        /// </summary>
        /// <returns>The path.</returns>
        public FPath MakeCurrent() {
            FPath.Current = this;
            return this;
        }

        /// <summary>
        /// Moves the current path in the file system.
        /// </summary>
        /// <param name="destination">The destination path.</param>
        /// <returns>The destination path.</returns>
        public FPath Move(FPath destination) {
            if (IsDirectory) {
                IO.Directory.Move(_path, destination.ToString());
            }
            else {
                IO.File.Move(_path, destination.ToString());
            }
            return destination;
        }

        /// <summary>
        /// Moves the current path in the file system.
        /// </summary>
        /// <param name="destination">The destination path.</param>
        /// <returns>The destination path.</returns>
        public FPath Move(string destination) {
            if (IsDirectory) {
                IO.Directory.Move(_path, destination);
            }
            else {
                IO.File.Move(_path, destination);
            }
            return new FPath(destination, this);
        }

        // File only APIs

        /// <summary>
        /// Opens the file at this path and hands it over to the specified action.
        /// </summary>
        /// <param name="action">The action to perform on the open stream.</param>
        /// <param name="mode">The FileMode to use. Default is OpenOrCreate.</param>
        /// <param name="access">The FileAccess to use. Default is ReadWrite.</param>
        /// <param name="share">The FileShare to use. Default is None.</param>
        /// <returns>The path.</returns>
        public FPath Open(Action<IO.FileStream> action, IO.FileMode mode = IO.FileMode.OpenOrCreate, IO.FileAccess access = IO.FileAccess.ReadWrite, IO.FileShare share = IO.FileShare.None) {
            using (var stream = IO.File.Open(_path, mode, access, share)) {
                action(stream);
            }
            return this;
        }

        /// <summary>
        /// Reads all the text in the file for this path.
        /// </summary>
        /// <returns>The text contents of the file.</returns>
        public string Read() {
            return IO.File.ReadAllText(_path);
        }

        /// <summary>
        /// Reads all the text in the file for this path.
        /// </summary>
        /// <param name="encoding">The encoding to use for reading the file.</param>
        /// <returns>The text contents of the file.</returns>
        public string Read(Encoding encoding) {
            return IO.File.ReadAllText(_path, encoding);
        }

        /// <summary>
        /// Reads all the contents of the file for this path.
        /// </summary>
        /// <returns>The binary contents of the file.</returns>
        public byte[] ReadBytes() {
            return IO.File.ReadAllBytes(_path);
        }

        /// <summary>
        /// Writes text into the file for this path.
        /// </summary>
        /// <param name="text">The text to write.</param>
        /// <param name="append">True if the text should be appended to the existing file. Default is false.</param>
        /// <returns>The path.</returns>
        public FPath Write(string text, bool append = false) {
            if (append) {
                IO.File.AppendAllText(_path, text);
            }
            else {
                IO.File.WriteAllText(_path, text);
            }
            return this;
        }

        /// <summary>
        /// Writes text into the file for this path.
        /// </summary>
        /// <param name="text">The text to write.</param>
        /// <param name="encoding">The encoding to use when writing to the file.</param>
        /// <param name="append">True if the text should be appended to the existing file. Default is false.</param>
        /// <returns>The path.</returns>
        public FPath Write(string text, Encoding encoding, bool append = false) {
            if (append) {
                IO.File.AppendAllText(_path, text, encoding);
            }
            else {
                IO.File.WriteAllText(_path, text, encoding);
            }
            return this;
        }

        /// <summary>
        /// Writes binary contents into the file for this path.
        /// </summary>
        /// <param name="bytes">The bytes to write.</param>
        /// <returns>The path.</returns>
        public FPath WriteBytes(byte[] bytes) {
            IO.File.WriteAllBytes(_path, bytes);
            return this;
        }

        /// <summary>
        /// Decrypts the file for this path.
        /// </summary>
        /// <returns>The path.</returns>
        public FPath Decrypt() {
            IO.File.Decrypt(_path);
            return this;
        }

        /// <summary>
        /// Encrypts the file for this path.
        /// </summary>
        /// <returns>The path.</returns>
        public FPath Encrypt() {
            IO.File.Encrypt(_path);
            return this;
        }

        // Fluent setters

        /// <summary>
        /// Sets the access control security for this path.
        /// </summary>
        /// <param name="security">The access control security.</param>
        /// <returns>The path.</returns>
        public FPath SetAccessControl(FileSystemSecurity security) {
            AccessControl = security;
            return this;
        }

        /// <summary>
        /// Sets the attributes for this path.
        /// </summary>
        /// <param name="attributes">The attributes to set.</param>
        /// <returns>The path.</returns>
        public FPath SetAttributes(IO.FileAttributes attributes) {
            Attributes = attributes;
            return this;
        }

        /// <summary>
        /// Sets the creation time for this path.
        /// </summary>
        /// <param name="creationTime">The time to set.</param>
        /// <returns>The path.</returns>
        public FPath SetCreationTime(DateTime creationTime) {
            CreationTime = creationTime;
            return this;
        }


        /// <summary>
        /// Sets the UTC creation time for this path.
        /// </summary>
        /// <param name="creationTime">The time to set.</param>
        /// <returns>The path.</returns>
        public FPath SetCreationTimeUtc(DateTime creationTimeUtc) {
            CreationTimeUtc = creationTimeUtc;
            return this;
        }

        /// <summary>
        /// Sets the last access time for this path.
        /// </summary>
        /// <param name="creationTime">The time to set.</param>
        /// <returns>The path.</returns>
        public FPath SetLastAccessTime(DateTime lastAccessTime) {
            LastAccessTime = lastAccessTime;
            return this;
        }

        /// <summary>
        /// Sets the UTC last access time for this path.
        /// </summary>
        /// <param name="creationTime">The time to set.</param>
        /// <returns>The path.</returns>
        public FPath SetLastAccessTimeUtc(DateTime lastAccessTimeUtc) {
            LastAccessTimeUtc = lastAccessTimeUtc;
            return this;
        }

        /// <summary>
        /// Sets the last write time for this path.
        /// </summary>
        /// <param name="creationTime">The time to set.</param>
        /// <returns>The path.</returns>
        public FPath SetLastWriteTime(DateTime lastWriteTime) {
            LastWriteTime = lastWriteTime;
            return this;
        }

        /// <summary>
        /// Sets the UTC last write time for this path.
        /// </summary>
        /// <param name="creationTime">The time to set.</param>
        /// <returns>The path.</returns>
        public FPath SetLastWriteTimeUtc(DateTime lastWriteTimeUtc) {
            LastWriteTimeUtc = lastWriteTimeUtc;
            return this;
        }
    }
}
