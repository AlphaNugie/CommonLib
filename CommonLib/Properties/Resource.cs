using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace YsuSoftHelper.Properties
{
	// Token: 0x02000038 RID: 56
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resource
	{
		// Token: 0x060001EF RID: 495 RVA: 0x00006F42 File Offset: 0x00005142
		internal Resource()
		{
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x060001F0 RID: 496 RVA: 0x00009C3C File Offset: 0x00007E3C
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				bool flag = Resource.resourceMan == null;
				if (flag)
				{
					ResourceManager resourceManager = new ResourceManager("YsuSoftHelper.Properties.Resource", typeof(Resource).Assembly);
					Resource.resourceMan = resourceManager;
				}
				return Resource.resourceMan;
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x060001F1 RID: 497 RVA: 0x00009C84 File Offset: 0x00007E84
		// (set) Token: 0x060001F2 RID: 498 RVA: 0x00009C9B File Offset: 0x00007E9B
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resource.resourceCulture;
			}
			set
			{
				Resource.resourceCulture = value;
			}
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x060001F3 RID: 499 RVA: 0x00009CA4 File Offset: 0x00007EA4
		internal static string FieldMeasureText
		{
			get
			{
				return Resource.ResourceManager.GetString("FieldMeasureText", Resource.resourceCulture);
			}
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x060001F4 RID: 500 RVA: 0x00009CCC File Offset: 0x00007ECC
		internal static string FieldSeparator
		{
			get
			{
				return Resource.ResourceManager.GetString("FieldSeparator", Resource.resourceCulture);
			}
		}

		// Token: 0x040000DF RID: 223
		private static ResourceManager resourceMan;

		// Token: 0x040000E0 RID: 224
		private static CultureInfo resourceCulture;
	}
}
