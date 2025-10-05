using Backend_AIHost.Models;

namespace Backend_AIHost.Data
{
    public static class SeedData
    {
           public static void SeedAIModel(AppDbContext _context)
        {
            if (!_context.AIModels.Any())
            {
                _context.AIModels.AddRange(
                    new AIModel
                    {
                        Id = 1,
                        MinRequiredRamMb = 1000,
                        Name = "DistilGPT2",
                        Description = "Lekki model czatowy oparty na GPT2, idealny do MVP i testów lokalnych.",
                        ModelInternalName = "distilgpt2",
                        Slug = "distilgpt2",
                        DefaultPort = 5000,
                        SupportsGPU = false,
                        IsActive = true,
                    },
    new AIModel
    {
        Id = 2,
        MinRequiredRamMb = 1300,
        Name = "GPT-2",
        Description = "Klasyczny model GPT-2 od OpenAI, większy niż DistilGPT2, ale wciąż lekki.",
        ModelInternalName = "gpt2",
        Slug = "gpt2",
        DefaultPort = 5000,
        SupportsGPU = false,
        IsActive = true
    },
    new AIModel
    {
        Id = 3,
        MinRequiredRamMb = 1600,
        Name = "GPT-Neo 125M",
        Description = "Model od EleutherAI będący lekką alternatywą dla GPT2. Nadaje się do prostych zadań konwersacyjnych.",
        ModelInternalName = "EleutherAI/gpt-neo-125M",
        Slug = "gpt-neo-125m",
        DefaultPort = 5000,
        SupportsGPU = false,
        IsActive = true
    },
    new AIModel
    {
        Id = 4,
        MinRequiredRamMb = 1800,
        Name = "Falcon-RW-1B",
        Description = "Niewielki model konwersacyjny od TII UAE, bardzo nowoczesna architektura.",
        ModelInternalName = "tiiuae/falcon-rw-1b",
        Slug = "falcon-rw-1b",
        DefaultPort = 5000,
        SupportsGPU = false,
        IsActive = true
    },
    new AIModel
    {
        Id = 5,
        MinRequiredRamMb = 1100,
        Name = "GPT-2 (OpenAI Community)",
        Description = "Otwarta wersja GPT2 od społeczności OpenAI, dobra do testów lokalnych.",
        ModelInternalName = "openai-community/gpt2",
        Slug = "openai-community-gpt2",
        DefaultPort = 5000,
        SupportsGPU = false,
        IsActive = true
    },
    new AIModel
    {
        Id = 6,
        MinRequiredRamMb = 1700,
        Name = "OPT-125M",
        Description = "Model z rodziny OPT od Meta AI. Bardzo lekki, idealny do eksperymentów.",
        ModelInternalName = "facebook/opt-125m",
        Slug = "opt-125m",
        DefaultPort = 5000,
        SupportsGPU = false,
        IsActive = true
    },
    new AIModel
    {
        Id = 7,
        MinRequiredRamMb = 7000,
        Name = "Mistral-7B-Instruct",
        Description = "Potężny model instrukcyjny (chatbotowy), działa dobrze ale wymaga dużo RAM-u.",
        ModelInternalName = "mistralai/Mistral-7B-Instruct-v0.1",
        Slug = "mistral-7b-instruct",
        DefaultPort = 5000,
        SupportsGPU = true,
        IsActive = true
    }
                );
                _context.SaveChanges();
            }
        }

    }
}
