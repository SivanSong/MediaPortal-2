#region Copyright (C) 2007-2017 Team MediaPortal

/*
    Copyright (C) 2007-2017 Team MediaPortal
    http://www.team-mediaportal.com

    This file is part of MediaPortal 2

    MediaPortal 2 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    MediaPortal 2 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MediaPortal 2. If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MediaPortal.Common.UserProfileDataManagement
{
  /// <summary>
  /// Data object for a named user profile.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Note: This class is serialized/deserialized by the <see cref="XmlSerializer"/>.
  /// If changed, this has to be taken into consideration.
  /// </para>
  /// </remarks>
  public class UserProfile
  {
    public const int CLIENT_PROFILE = 0;
    public const int USER_PROFILE = 1;
    public const int ADMIN_PROFILE = 100;

    protected Guid _profileId;
    protected string _name;
    protected string _password;
    protected byte[] _image;
    protected DateTime? _lastLogin;
    protected int _profileType;

    // We could use some cache for this instance, if we would have one...
    protected static XmlSerializer _xmlSerializer = null; // Lazy initialized

    public UserProfile(Guid profileId, string name, int profileType = CLIENT_PROFILE, string password = null, byte[] image = null, DateTime? lastLogin = null)
    {
      _profileId = profileId;
      _name = name;
      _password = password;
      _image = image;
      _lastLogin = lastLogin;
      _profileType = profileType;
    }

    /// <summary>
    /// Returns the globally unique id of this user profile.
    /// </summary>
    [XmlIgnore]
    public Guid ProfileId
    {
      get { return _profileId; }
    }

    /// <summary>
    /// Returns the user name of this profile.
    /// </summary>
    [XmlIgnore]
    public string Name
    {
      get { return _name; }
    }

    /// <summary>
    /// Returns the type this profile.
    /// </summary>
    [XmlIgnore]
    public int ProfileType
    {
      get { return _profileType; }
    }

      /// <summary>
      /// Returns the hashed password of this profile.
      /// </summary>
      [XmlIgnore]
    public string Password
    {
      get { return _password; }
    }

    /// <summary>
    /// Returns the user image of this profile.
    /// </summary>
    [XmlIgnore]
    public byte[] Image
    {
      get { return _image; }
    }

    /// <summary>
    /// Returns the last login of this profile.
    /// </summary>
    [XmlIgnore]
    public DateTime? LastLogin
    {
      get { return _lastLogin; }
    }

    /// <summary>
    /// Serializes this user profile instance to XML.
    /// </summary>
    /// <returns>String containing an XML fragment with this instance's data.</returns>
    public string Serialize()
    {
      XmlSerializer xs = GetOrCreateXMLSerializer();
      StringBuilder sb = new StringBuilder(); // Will contain the data, formatted as XML
      using (XmlWriter writer = XmlWriter.Create(sb, new XmlWriterSettings {OmitXmlDeclaration = true}))
        xs.Serialize(writer, this);
      return sb.ToString();
    }

    /// <summary>
    /// Serializes this user profile instance to the given <paramref name="writer"/>.
    /// </summary>
    /// <param name="writer">Writer to write the XML serialization to.</param>
    public void Serialize(XmlWriter writer)
    {
      XmlSerializer xs = GetOrCreateXMLSerializer();
      xs.Serialize(writer, this);
    }

    /// <summary>
    /// Deserializes a user profile instance from a given XML fragment.
    /// </summary>
    /// <param name="str">XML fragment containing a serialized user profile instance.</param>
    /// <returns>Deserialized instance.</returns>
    public static UserProfile Deserialize(string str)
    {
      XmlSerializer xs = GetOrCreateXMLSerializer();
      using (StringReader reader = new StringReader(str))
        return xs.Deserialize(reader) as UserProfile;
    }

    /// <summary>
    /// Deserializes a user profile instance from a given <paramref name="reader"/>.
    /// </summary>
    /// <param name="reader">XML reader containing a serialized user profile instance.</param>
    /// <returns>Deserialized instance.</returns>
    public static UserProfile Deserialize(XmlReader reader)
    {
      XmlSerializer xs = GetOrCreateXMLSerializer();
      return xs.Deserialize(reader) as UserProfile;
    }

    #region Base overrides

    public override bool Equals(object obj)
    {
      if (!(obj is UserProfile))
        return false;
      UserProfile other = (UserProfile) obj;
      return _profileId == other._profileId;
    }

    public override int GetHashCode()
    {
      return _profileId.GetHashCode();
    }

    public override string ToString()
    {
      return string.Format("User profile {0}: Name='{1}'", _profileId, _name);
    }

    #endregion

    #region Additional members for the XML serialization

    internal UserProfile() { }

    protected static XmlSerializer GetOrCreateXMLSerializer()
    {
      return _xmlSerializer ?? (_xmlSerializer = new XmlSerializer(typeof(UserProfile)));
    }

    /// <summary>
    /// For internal use of the XML serialization system only.
    /// </summary>
    [XmlAttribute("Id")]
    public Guid XML_Id
    {
      get { return _profileId; }
      set { _profileId = value; }
    }

    /// <summary>
    /// For internal use of the XML serialization system only.
    /// </summary>
    [XmlAttribute("Name")]
    public string XML_Name
    {
      get { return _name; }
      set { _name = value; }
    }

    /// <summary>
    /// For internal use of the XML serialization system only.
    /// </summary>
    [XmlAttribute("ProfileType")]
    public int XML_ProfileType
    {
      get { return _profileType; }
      set { _profileType = value; }
    }

    /// <summary>
    /// For internal use of the XML serialization system only.
    /// </summary>
    [XmlAttribute("Password")]
    public string XML_Password
    {
      get { return _password; }
      set { _password = value; }
    }

    /// <summary>
    /// For internal use of the XML serialization system only.
    /// </summary>
    [XmlAttribute("Image")]
    public string XML_Image
    {
      get { return _image != null && _image.Length > 0 ? Convert.ToBase64String(_image) : ""; }
      set { _image = string.IsNullOrEmpty(value) ? null : Convert.FromBase64String(value); }
    }

    /// <summary>
    /// For internal use of the XML serialization system only.
    /// </summary>
    [XmlAttribute("LastLogin")]
    public long XML_LastLogin
    {
      get { return _lastLogin.HasValue ? _lastLogin.Value.Ticks : 0; }
      set { _lastLogin = value == 0 ? (DateTime?)null : new DateTime(value); }
    }

    #endregion
  }
}
