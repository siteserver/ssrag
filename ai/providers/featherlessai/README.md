## featherlessai

**Author:** darinverheijke
**Version:** 0.0.1
**Type:** model

### Description

[Featherless AI](https://featherless.ai) is a serverless inference provider that gives access to thousands of open source models such as DeepSeek, Qwen, Llama, Mistral and more.

#### Step-by-step setup instructions

To configure your [Featherless AI](https://featherless.ai) provider, follow these steps:

##### Step 1: Select Model Type
- The **Model Type** field should already be set to "LLM"
- This is typically pre-selected and doesn't need to be changed

##### Step 2: Enter Model Name
- In the **Model Name** field, enter the full model name exactly as it appears in Featherless AI
- You can find the list of models on https://featherless.ai/models 
- Examples of available models:
  - `deepseek-ai/DeepSeek-V3-0324`
  - `mistralai/Mistral-Nemo-Instruct-2407`
  - `meta-llama/Meta-Llama-3.1-8B-Instruct`
  - `Qwen/Qwen3-32B`
  - `THUD/GLM-4-32B-0414`
  - `Qwen/QwQ-32B`

##### Step 3: Add Your API Key
- In the **API Key** field, enter your Featherless AI API key
- If you don't have one, visit [featherless.ai](https://featherless.ai) to get your API key

##### Step 4: Set Completion Mode
- The **Completion mode** should be set to "Chat"
- This is the standard mode for most language models

##### Step 5: Configure Context Size
- Set the **Model context size** to the appropriate value for your chosen model
- Common values include `16384`, `32768`
- Check the model page for the correct context size

##### Step 6: Set Token Limit
- Set the **Upper bound for max tokens**
- This should typically match or be less than the context size
- Example: `4096` for models with 4K context

##### Step 7: Save the Configuration
- Click the **Save** button to add the model to your configuration
- Click **Cancel** if you want to abort the process

##### Notes:
- Make sure you have a subscription on your Featherless AI account
- The model name must match exactly what's available in the Featherless AI catalog
- Some models may have different context sizes - check the official documentation for accurate values
- All models support chat completion mode for conversational AI applications


