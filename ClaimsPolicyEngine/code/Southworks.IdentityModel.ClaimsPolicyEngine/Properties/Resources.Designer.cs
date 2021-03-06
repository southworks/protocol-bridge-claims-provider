﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4013
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Southworks.IdentityModel.ClaimsPolicyEngine.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Southworks.IdentityModel.ClaimsPolicyEngine.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The claim type &apos;{0}&apos; was not found on the claimTypes section of the scope..
        /// </summary>
        internal static string ClaimTypeNotDefined {
            get {
                return ResourceManager.GetString("ClaimTypeNotDefined", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Output claims cannot have copyFromInput on &apos;true&apos; and a value set..
        /// </summary>
        internal static string CopyFromInputAndValueSet {
            get {
                return ResourceManager.GetString("CopyFromInputAndValueSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Output claims cannot have copyFromInput on &apos;true&apos; when multiple input claims are defined on the mapping rule..
        /// </summary>
        internal static string CopyFromInputWithMultipleInputClaims {
            get {
                return ResourceManager.GetString("CopyFromInputWithMultipleInputClaims", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The issuer &apos;{0}&apos; was not found on the issuers section of the scope..
        /// </summary>
        internal static string IssuerNotDefined {
            get {
                return ResourceManager.GetString("IssuerNotDefined", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Issuer property of the Claim cannot be null..
        /// </summary>
        internal static string IssuerNotNull {
            get {
                return ResourceManager.GetString("IssuerNotNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Output claims must have either copyFromInput on &apos;true&apos; or a value set..
        /// </summary>
        internal static string NoOutputValueSet {
            get {
                return ResourceManager.GetString("NoOutputValueSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A scope with uri &apos;{0}&apos; was found repeated on the Store. Scope&apos;s URIs must be unique for the entire store..
        /// </summary>
        internal static string RepeatedScope {
            get {
                return ResourceManager.GetString("RepeatedScope", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Scope &apos;{0}&apos; was not found on the policy store..
        /// </summary>
        internal static string ScopeNotFound {
            get {
                return ResourceManager.GetString("ScopeNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Output claims cannot have &apos;*&apos; wildcard as value..
        /// </summary>
        internal static string WildcardOnOutputClaim {
            get {
                return ResourceManager.GetString("WildcardOnOutputClaim", resourceCulture);
            }
        }
    }
}
