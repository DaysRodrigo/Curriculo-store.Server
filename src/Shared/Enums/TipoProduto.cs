//using System;

//namespace Curriculo_store.Server.Shared.Enums
//{
//    public enum TipoProduto
//    {
//        Curso,
//        Experiência,
//        Acadêmico,
//        Outro
//    }
//}

using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Curriculo_store.Server.Shared.Enums
{
    public enum TipoProduto
    {
        [Display(Name = "Curso")]
        Curso,

        [Display(Name = "Experiência")]
        Experiencia,

        [Display(Name = "Acadêmico")]
        Academico,

        [Display(Name = "Outro")]
        Outro
    }

    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())[0]
                            .GetCustomAttribute<DisplayAttribute>()?
                            .Name ?? enumValue.ToString();
        }
    }
}