# vllm-openai provider plugin to support guided generate

## **NOTE!!!**
**This plugin is a extension for official OpenAI-API-compatible,** 
**provide features for [extra parameters in vLLM's OpenAI-Compatible Server](https://docs.vllm.ai/en/latest/serving/openai_compatible_server.html#extra-parameters).**
**Only extra parameters featured model will be implemented here, Please use official OpenAI-API-compatible you don't have such needs.**

**本插件是在官方 OpenAI-API-compatible 基础上构建, 用于提供[vLLM's OpenAI-Compatible Server 中的 extra parameters](https://docs.vllm.ai/en/latest/serving/openai_compatible_server.html#extra-parameters)**
**的调用(现主要集中在 CFG 即 Guided Generate 上). 若没有使用上述 extra parameters 中的相关的特性, 请使用官方 OpenAI-API-compatible 插件.**
