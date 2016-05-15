namespace Moriyama.UmbracoFlatten.Lib.Sql
{
    public class Constants
    {
        public const string GetAllPropertyAliasesSql = @"SELECT DISTINCT
  cmsPropertyType.Alias AS Alias
FROM umbracoNode,
     cmsPropertyData,
     cmsDocument,
     cmsPropertyType
WHERE nodeObjectType = 'C66BA18E-EAF3-4CFF-8A22-41B16D66A972'
AND umbracoNode.id = cmsPropertyData.contentNodeId
AND cmsDocument.nodeId = umbracoNode.Id
AND cmsPropertyType.id = cmsPropertyData.propertytypeid
AND cmsDocument.newest = 1";

        public const string GetAllPropertyValuesForAlias = @"SELECT
  COALESCE(CONVERT(nvarchar(max), dataInt), '') +
  COALESCE(CONVERT(nvarchar(max), dataDecimal), '') +
  COALESCE(CONVERT(nvarchar(max), dataDate), '') +
  COALESCE(CONVERT(nvarchar(max), dataNvarchar), '') +
  COALESCE(CONVERT(nvarchar(max), dataNText), '')
FROM umbracoNode,
     cmsPropertyData,
     cmsDocument,
     cmsPropertyType
WHERE nodeObjectType = 'C66BA18E-EAF3-4CFF-8A22-41B16D66A972'
AND umbracoNode.id = cmsPropertyData.contentNodeId
AND cmsDocument.nodeId = umbracoNode.Id
AND cmsPropertyType.id = cmsPropertyData.propertytypeid
AND cmsDocument.newest = 1
AND cmsPropertyType.Alias = @0";

        public const string GetAllContentIds = @"SELECT DISTINCT
  (cmsDocument.nodeId)
FROM umbracoNode,
     cmsPropertyData,
     cmsDocument,
     cmsPropertyType
WHERE nodeObjectType = 'C66BA18E-EAF3-4CFF-8A22-41B16D66A972'
AND umbracoNode.id = cmsPropertyData.contentNodeId
AND cmsDocument.nodeId = umbracoNode.Id
AND cmsPropertyType.id = cmsPropertyData.propertytypeid
AND cmsDocument.newest = 1";

        public const string GetAllValuesForContent = @"SELECT
  COALESCE(CONVERT(nvarchar(max), dataInt), '') +
  COALESCE(CONVERT(nvarchar(max), dataDecimal), '') +
  COALESCE(CONVERT(nvarchar(max), dataDate), '') +
  COALESCE(CONVERT(nvarchar(max), dataNvarchar), '') +
  COALESCE(CONVERT(nvarchar(max), dataNText), '') AS Value,
  cmsPropertyType.Alias
FROM umbracoNode,
     cmsDocument,
     cmsPropertyData,
     cmsPropertyType
WHERE cmsDocument.nodeId = @0
AND cmsDocument.newest = 1
AND cmsDocument.nodeId = umbraconode.id
AND cmsDocument.versionId = cmsPropertyData.versionId
AND cmsPropertyData.contentNodeId = umbraconode.id
AND cmsPropertyData.propertytypeid = cmsPropertyType.id

AND cmsPropertyData.contentNodeId = cmsDocument.nodeId
AND umbracoNode.id = cmsDocument.nodeId
AND umbracoNode.id = cmsPropertyData.contentNodeId";


    }
}
