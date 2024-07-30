using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace App.Extensions;

public static class KernelExtensions
{
    private const string Locations = "alentejo, algarve, aveiro, azores, braga, braganca, coimbra, covilha, funchal, guimaraes, leiria, lisbon, porto, santarem, vila_real, viseu";
    
    public static KernelFunction GetLocationFunction(this Kernel kernel)
    {
        return kernel.CreateFunctionFromPrompt(
            new PromptTemplateConfig()
            {
                Name = "GetLocation",
                Description = "Extract location from user prompt.",
                Template = $@"
                    <message role=""system"">Extract the location from the user input. Choose one of the following values: {Locations}</message>

                    For example: 

                    <message role=""user"">I'm in Porto, where should I eat?</message>
                    <message role=""assistant"">porto</message>

                    <message role=""user"">{{{{$input}}}}</message>",
                TemplateFormat = "semantic-kernel",
                InputVariables =
                [
                    new() { Name = "input", Description = "Text asking for suggestions about some location", IsRequired = true },
                ],
                ExecutionSettings =
                {
                    {
                        "default",
                        new OpenAIPromptExecutionSettings()
                        {
                            MaxTokens = 5,
                            Temperature = 0,
                            StopSequences = [
                                "</s>",
                                "Llama:",
                                "User:"
                            ]
                        }
                    }
                }
            }
        );
    }
}