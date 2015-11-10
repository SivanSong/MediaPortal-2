﻿using System;
using System.Collections.Generic;
using System.Linq;
using HttpServer;
using HttpServer.Exceptions;
using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.Common.MediaManagement;
using MediaPortal.Common.MediaManagement.DefaultItemAspects;
using MediaPortal.Plugins.MP2Extended.Attributes;
using MediaPortal.Plugins.MP2Extended.Common;
using MediaPortal.Plugins.MP2Extended.Extensions;
using MediaPortal.Plugins.MP2Extended.MAS;
using MediaPortal.Plugins.MP2Extended.MAS.General;
using MediaPortal.Plugins.MP2Extended.MAS.Movie;
using MediaPortal.Plugins.MP2Extended.MAS.Picture;
using Newtonsoft.Json;

namespace MediaPortal.Plugins.MP2Extended.ResourceAccess.MAS.Picture
{
  [ApiFunctionDescription(Type = ApiFunctionDescription.FunctionType.Json, Summary = "")]
  [ApiFunctionParam(Name = "id", Type = typeof(string), Nullable = false)]
  internal class GetPictureDetailedById : IRequestMicroModuleHandler
  {
    public dynamic Process(IHttpRequest request)
    {
      HttpParam httpParam = request.Param;
      string id = httpParam["id"].Value;

      if (id == null)
        throw new BadRequestException("GetPictureDetailedById: id is null");

      Guid idGuid;
      if (!Guid.TryParse(id, out idGuid))
        throw new BadRequestException(string.Format("GetPictureDetailedById: couldn't parse id '{0}' to Guid", id));

      ISet<Guid> necessaryMIATypes = new HashSet<Guid>();
      necessaryMIATypes.Add(MediaAspect.ASPECT_ID);
      necessaryMIATypes.Add(ProviderResourceAspect.ASPECT_ID);
      necessaryMIATypes.Add(ImporterAspect.ASPECT_ID);
      necessaryMIATypes.Add(ImageAspect.ASPECT_ID);

      MediaItem item = GetMediaItems.GetMediaItemById(idGuid, necessaryMIATypes);

      if (item == null)
        throw new BadRequestException(string.Format("No Image found with id: {0}", id));

        MediaItemAspect imageAspects = item.Aspects[ImageAspect.ASPECT_ID];

        WebPictureDetailed webPictureDetailed = new WebPictureDetailed();

      //webPictureBasic.Categories = imageAspects.GetAttributeValue(ImageAspect);
      //webPictureBasic.DateTaken = imageAspects.GetAttributeValue(ImageAspect.);
      webPictureDetailed.Type = WebMediaType.Picture;
      //webPictureBasic.Artwork;
      webPictureDetailed.DateAdded = (DateTime)item.Aspects[ImporterAspect.ASPECT_ID].GetAttributeValue(ImporterAspect.ATTR_DATEADDED);
      webPictureDetailed.Id = item.MediaItemId.ToString();
      webPictureDetailed.PID = 0;
      //webPictureBasic.Path;
      webPictureDetailed.Title = (string)item.Aspects[MediaAspect.ASPECT_ID].GetAttributeValue(MediaAspect.ATTR_TITLE);
      //webPictureDetailed.Rating = imageAspects.GetAttributeValue(ImageAspect.);
      //webPictureDetailed.Author = imageAspects.GetAttributeValue(ImageAspect.);
      //webPictureDetailed.Dpi = imageAspects.GetAttributeValue(ImageAspect.);
      webPictureDetailed.Width = (string)(imageAspects.GetAttributeValue(ImageAspect.ATTR_WIDTH) ?? string.Empty);
      webPictureDetailed.Height = (string)(imageAspects.GetAttributeValue(ImageAspect.ATTR_HEIGHT) ?? string.Empty);
      //webPictureDetailed.Mpixel = imageAspects.GetAttributeValue(ImageAspect.);
      //webPictureDetailed.Copyright;
      webPictureDetailed.CameraModel = (string)(imageAspects.GetAttributeValue(ImageAspect.ATTR_MODEL) ?? string.Empty);
      webPictureDetailed.CameraManufacturer = (string)(imageAspects.GetAttributeValue(ImageAspect.ATTR_MAKE) ?? string.Empty);
      //webPictureDetailed.Comment;
      //webPictureDetailed.Subject;

      return webPictureDetailed;
    }

    internal static ILogger Logger
    {
      get { return ServiceRegistration.Get<ILogger>(); }
    }
  }
}