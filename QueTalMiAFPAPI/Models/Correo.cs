using Amazon.Lambda.Model;

namespace QueTalMiAFPAPI.Models {
    public class Correo {
        public DireccionCorreo? De { get; set; } = null;
        public required List<DireccionCorreo> Para { get; set; }
        public List<DireccionCorreo>? Cc { get; set; } = null;
        public List<DireccionCorreo>? Cco { get; set; } = null;
        public List<DireccionCorreo>? ResponderA { get; set; } = null;
        public required string Asunto { get; set; }
        public required string Cuerpo { get; set; }
        public List<Adjunto>? Adjuntos { get; set; } = null;
    }

    public class DireccionCorreo {
        public string? Nombre { get; set; }
        public required string Correo { get; set; }
        public override string ToString() {
            if (Nombre != null) {
                return $"\"{Nombre}\" <{Correo}>";
            }
            return Correo;
        }
    }

    public class Adjunto {
        public required string NombreArchivo { get; set; }
        public required string TipoMime { get; set; }
        public required string ContenidoBase64 { get; set; }
    }
}
