using Microsoft.Extensions.Primitives;

namespace Backend_AIHost.Models
{
    public class UserImage
    {
        public int Id { get; set; }
        public int VPSId { get; set; } // ID VPS, na którym jest obraz
        public int ModelId { get; set; } // ID modeilu AI
        public int SizeMb { get; set; } // Rozmiar obrazu w MB
        public int ExposePort { get; set; } // Port wystawiony przez obraz
        public string ImageId { get; set; } // ID obrazu w Dockerze (np. "e1ab2fcdb204")
        public string ImageName { get; set; } // Nazwa obrazu
        public string ModelName { get; set; } // Nazwa modelu AI, jeśli obraz jest oparty na modelu
        public string Tag { get; set; } // Tag obrazu (np. wersja)
        public string UserId { get; set; } // ID użytkownika, do którego należy obraz
        public DateTime CreatedAt { get; set; } // Data utworzenia obrazu
        public bool IsActive { get; set; } // Czy obraz jest aktywny (używany w kontenerach)

    }
}
