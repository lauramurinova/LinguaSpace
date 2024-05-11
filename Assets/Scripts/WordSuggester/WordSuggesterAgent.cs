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
    public void Awake()
    {
        var openAIKey = GetApiKey();
        if (openAIKey.Length == 0)
        {
            throw new SystemException("Open AI Api key file not found in project: Resources/Security/OpenAI-APIKey.txt");
        }
        openai = new OpenAIApi(openAIKey);
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

    // async methods can only return limited data types
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
            Debug.Log($"Server response: {completionResponse}");
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
            Debug.Log(line);
        }
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
            You are a thesaurus. Give me {numWords} in {toLanguage} from category:{toCategory} 
            and Difficulty:{difficultyString} commonly used with the {fromLanguage} word {fromWord}.  
            Present the results as a plain text file with two comma-separated columns: word in {fromLanguage}, 
            word in {toLanguage} and nothing more.";
        String customMixGptQuery = $@"
            You are a thesaurus. Give me {numWords} words in {toLanguage} 
            commonly used with the {fromLanguage} word {fromWord}.  
            One of them should be a Synonym, One of them should be a Verb, One of them should be an Adjective.
            Do not include the original word in the list of 3. 
            Present the results as a plain text file with only two comma-separated columns: 
            word in {fromLanguage}, that {fromLanguage} word translated in {toLanguage} and nothing more. Do not prefix anything";
        String anyGptQuery = $@"
            You are a thesaurus. Give me {numWords} words in {toLanguage} 
            commonly used with the {fromLanguage} word {fromWord}.  
            Do not include the original word in the list of 3. 
            Present the results as a plain text file with only two comma-separated columns: 
            word in {fromLanguage}, that {fromLanguage} word translated in {toLanguage} and nothing more. Do not prefix anything";

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
