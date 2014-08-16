using System;
using MFR.Client.MFR.Client;
using Newtonsoft.Json.Linq;

namespace BSSPlugin.Command
{
    public class HashHelper
    {
        private static long GetHashCode(JObject someObject)
        {
            var hashKeyValue = (long)1;

            WalkNode(someObject, n =>
                {
                    if (n.Type == JTokenType.Null || n.Type == JTokenType.Date)
                        return;

                    if (n.Type == JTokenType.Boolean && n.Value<bool>() == false)
                        return;

                    var value = n.Value<string>();
                    if (string.IsNullOrEmpty(value) || value == "01.01.0001 00:00:00" || value == "1/1/0001 12:00:00 AM")
                        return;

                    hashKeyValue = hashKeyValue + value.GetHashCode();
            });

            return hashKeyValue;
        }

        private static void WalkNode(JToken node, Action<JToken> action)
        {
            if (node.Type == JTokenType.Object)
            {
                foreach (JProperty child in node.Children<JProperty>())
                {
                    WalkNode(child.Value, action);
                }
            }
            else if (node.Type == JTokenType.Array)
            {
                foreach (var child in node.Children())
                {
                    WalkNode(child, action);
                }
            }
            else
            {
                action((JToken)node);
            }
        }

        public static long GetHashValueCode(Step step)
        {
            unchecked
            {
                long hashCode = (step.Name != null ? step.Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (step.Type != null ? step.Type.GetHashCode() : 0);

                try
                {
                    hashCode = (hashCode * 397) ^ (step.Data != null ? GetHashCode(JObject.Parse(step.Data)) : 0);
                }
                catch (Exception)
                {
                    
                }

                hashCode = (hashCode * 397) ^ (step.Description != null ? step.Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (step.ParentId != null ? step.ParentId.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static int GetHashValueCode(ServiceRequest serviceRequest)
        {
            return 0;
        }

        public static long GetHashValueCode(ServiceObject serviceObject)
        {
            unchecked
            {
                long hashCode = (serviceObject.Name != null ? serviceObject.Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (serviceObject.Note != null ? serviceObject.Note.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ serviceObject.DateModified.GetHashCode();
                hashCode = (hashCode * 397) ^ (serviceObject.ExternalId != null ? serviceObject.ExternalId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (serviceObject.MappingId != null ? serviceObject.MappingId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (serviceObject.Location != null ? serviceObject.Location.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ serviceObject.ParentServiceObjectId.GetHashCode();
                hashCode = (hashCode * 397) ^ (serviceObject.CompanyId != null ? serviceObject.CompanyId.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static long GetHashValueCode(StepListTemplate stepListTemplate)
        {
            unchecked
            {
                long hashCode = (stepListTemplate.Name != null ? stepListTemplate.Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ stepListTemplate.IsReleased.GetHashCode();
                hashCode = (hashCode * 397) ^ stepListTemplate.IsDurable.GetHashCode();
                return hashCode;
            }
        }

        public static long GetHashValueCode(Product product)
        {
            unchecked
            {
                long hashCode = (product.Name != null ? product.Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (product.SubKey != null ? product.SubKey.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (product.Description != null ? product.Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (product.MappingId != null ? product.MappingId.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}