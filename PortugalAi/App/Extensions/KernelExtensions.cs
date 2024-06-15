using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace App.Extensions;

public static class KernelExtensions
{
    public static KernelFunction GetLocationFunction(this Kernel kernel)
    {
        return kernel.CreateFunctionFromPrompt(
            new PromptTemplateConfig()
            {
                Name = "GetLocation",
                Description = "Extract location from user prompt.",
                Template = @"
                    <message role=""system"">Identify the portuguese location</message>

                    For example: 

                    <message role=""user"">I'm in Porto, where should I eat?</message>
                    <message role=""assistant"">Porto</message>

                    <message role=""user"">What should I visit?</message>
                    <message role=""assistant"">null</message>

                    <message role=""user"">What are some of the best dishes in the north?</message>
                    <message role=""assistant"">North</message>

                    <message role=""user"">{{$input}}</message>",
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