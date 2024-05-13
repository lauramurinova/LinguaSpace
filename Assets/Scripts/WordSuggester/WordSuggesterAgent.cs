using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI;
using UnityEngine;
 
public class WordSuggesterAgent : MonoBehaviour
{
    private OpenAIApi openai;

    public enum WordCategory
    {
        CustomMix,
        Any,
        Synonym,
        Noun,
        Verb,
        Adjective
    };
    private string GetApiKey()
    {
        return Resources.Load<TextAsset>("Security/OpenAI-APIKey").ToString();
    }
    public void Start()
    {
        var openAIKey = GetApiKey();
        if (openAIKey.Length == 0)
        {
            throw new SystemException("Open AI Api key file not found in project: Resources/Security/OpenAI-APIKey.txt");
        }
        openai = new OpenAIApi(openAIKey);
        if (openai == null)
        {
            throw new SystemException("Could not initialze OpenAI API, check network and key");
        }
        Debug.Log($"openAI Created Succesfully: {openai}");
    }

    static String[] ConvertToStringArr(string input)
    {
        return input.Split('\n');
    }

    // we expect table results in this format
    static List<Tuple<string, string>> ConvertToTupleList(string input)
    {
        List<Tuple<string, string>> result = new List<Tuple<string, string>>();

        string[] lines = input.Split('\n');
        foreach (string line in lines)
        {
            string[] columns = line.Trim().Split(',');
            if (columns.Length == 2)
            {
                result.Add(Tuple.Create(columns[0], columns[1]));
            }
        }

        return result;
    }

    public async Task<String> callChatGpt(String queryText)
    {
        var newMessage = new ChatMessage()
        {
            Role = "user",
            Content = queryText
        };
        List<ChatMessage> newMessages = new List<ChatMessage>();
        newMessages.Add(newMessage);

        // Complete the instruction
        var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
        {
            Model = "gpt-3.5-turbo",
            Messages = newMessages
        });

        if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
        {
            var response = completionResponse.Choices[0].Message;
            // Debug.Log($"Server response: {completionResponse}");
            return response.Content;
        }
        else
        {
            Debug.LogWarning("No text was generated from this prompt.");
        }

        return "";
    }

    public String FormatMultiLineLiteral(String multiLineString)
    {
        // Remove leading and trailing whitespace from each line
        string[] lines = multiLineString.Trim().Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = lines[i].Trim();
        }

        // Join the lines back together with a single space between them
        return string.Join(" ", lines);
    }

    public void DebugMultiLineLog(String multilineString)
    {
        // Split the multiline string into individual lines
        string[] lines = multilineString.Split('\n');
        foreach (string line in lines)
        {
            // Debug.Log(line);
        }
    }
    
    public async Task<Tuple<String, String>> ConstructSentence(String wordInToLanguage, String toLanguage, 
        String fromLanguage, int minNumWords = 5, int maxNumWords = 10)
    {
        String gptQuery = $@"
            You are a language coach. Give me a sentence between {minNumWords} and {maxNumWords} words in {toLanguage} 
            using the word {wordInToLanguage}.  Then give me the same sentence translated into {fromLanguage}. 
            Do not add language labels. There will be two lines of text total in the output. 
            Terminate the first sentence with a single newline character. 
            ";
        
        var query = FormatMultiLineLiteral(gptQuery);
        DebugMultiLineLog($"Query: {query}");
        String responseString = await callChatGpt(query);
        DebugMultiLineLog($"Response: {responseString}");

        if (responseString.Length > 0)
        {
            var resultsDataTable = ConvertToStringArr(responseString);
            if (resultsDataTable.Length == 2)
            {
                return new Tuple<string, string>(resultsDataTable[0], resultsDataTable[1]);
            }
        }

        return null;
    }
    
    public async Task<List<Tuple<String, String>>> FindRelatedWords(String fromWord, String fromLanguage,
        String toLanguage, WordCategory toCategory = WordCategory.Any, int numWords = 3,
        float difficulty = 0.0f)
    {
        String difficultyString = "Beginner";
        if (difficulty >= 0.25f && difficulty <= 0.51f)
        {
            difficultyString = "Intermediate";
        }
        else if (difficulty < 0.51f)
        {
            difficultyString = "Advanced";
        }

        String standardGptQuery = $@"
            You are a language coach. Give me {numWords} single words with no spaces in {toLanguage} from category:{toCategory} 
            commonly used with the {fromLanguage} word {fromWord}.  
            Present the results as a plain text file with only two comma-separated columns: word in {fromLanguage}, 
            that {fromLanguage} word translated to {toLanguage} and nothing more. Each row contains
            exactly the two words and nothing more.  Do not prefix anything";
        String customMixGptQuery = $@"
            You are a thesaurus. Give me {numWords} words in {toLanguage} 
            commonly used with the {fromLanguage} word {fromWord}.  
            One of them should be a Synonym, One of them should be a Verb, One of them should be an Adjective.
            Do not include the original word in the list of 3. 
            Present the results as a plain text file with only two comma-separated columns: 
            word in {fromLanguage}, that {fromLanguage} word translated to {toLanguage} and nothing more. Do not prefix anything";
        String anyGptQuery = $@"
            You are a language coach. Give me {numWords} words in {toLanguage} 
            commonly used with the {fromLanguage} word {fromWord}.  
            Do not include the original word in the list of 3. 
            Present the results as a plain text file with only two comma-separated columns: 
            word in {fromLanguage}, that {fromLanguage} word translated to {toLanguage} and nothing more. Do not prefix anything";

        var query = standardGptQuery;
        if (toCategory == WordCategory.CustomMix)
        {
            query = customMixGptQuery;
        } else if (toCategory == WordCategory.Any)
        {
            query = anyGptQuery;
        }
        query = FormatMultiLineLiteral(query);
        DebugMultiLineLog($"Query: {query}");
        String responseString = await callChatGpt(query);
        DebugMultiLineLog($"Response: {responseString}");

        if (responseString.Length > 0)
        {
            var resultsDataTable = ConvertToTupleList(responseString);
            return resultsDataTable;
        }

        return null;
    }
}
