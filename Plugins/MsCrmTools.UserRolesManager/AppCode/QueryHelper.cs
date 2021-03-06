﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace MsCrmTools.UserRolesManager.AppCode
{
    internal class QueryHelper
    {
        public static EntityCollection GetItems(string fetchXml, IOrganizationService service)
        {
            // Check if businessunitid attribute is contained in attriburtes
            var xDoc = XDocument.Parse(fetchXml);
            AddMissingCrmAttribute(xDoc, "businessunitid");

            var entityElement = xDoc.Descendants("entity").FirstOrDefault();
            if (entityElement == null)
            {
                throw new Exception("Cannot find node 'entity' in FetchXml");
            }

            switch (entityElement.Attribute("name").Value)
            {
                case "systemuser":
                {
                    AddMissingCrmAttribute(xDoc, "firstname");
                    AddMissingCrmAttribute(xDoc, "lastname");
                }
                break;
                case "team":
                {
                    AddMissingCrmAttribute(xDoc, "name");
                }
                break;
            }

            return service.RetrieveMultiple(new FetchExpression(xDoc.ToString()));
        }

        private static void AddMissingCrmAttribute(XDocument xDoc, string attributeName)
        {
            var xBuAttribute = xDoc.XPathSelectElement("fetch/entity/attribute[@name='" + attributeName + "']");
            if (xBuAttribute == null)
            {
                var entityElement = xDoc.Descendants("entity").FirstOrDefault();
                if (entityElement == null)
                {
                    throw new Exception("Cannot find node 'entity' in FetchXml");
                }

                entityElement.Add(new XElement("attribute", new XAttribute("name", attributeName)));
            }
        }
    }
}
