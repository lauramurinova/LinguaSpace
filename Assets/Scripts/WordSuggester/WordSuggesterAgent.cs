using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI;
using UnityEngine;
 
public class WordSuggesterAgent : MonoBehaviour
{
    private OpenAIApi openai = new OpenAIApi();

    public enum WordCategory
    {
        Any,
        Synonym,
        Noun,
        Verb,
        Adjective
    };
    
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
        // XXX I don't think we need message history, just send the single query directly
        //AppendMessage(newMessage);
        //if (messages.Count == 0) newMessage.Content = prompt + "\n" + inputField.text;
        //messages.Add(newMessage);

        // Complete the instruction
        var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
        {
            Model = "gpt-3.5-turbo-1106",
            Messages = newMessages
        });

        if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
        {
            var response = completionResponse.Choices[0].Message;
            //response.Content = response.Content.Trim();
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
        String toLanguage, WordCategory toCategory = WordCategory.Any, int numWords = 5,
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

        String findRelatedWordsGptQuery = $@"
            You are a thesaurus. Give me {numWords} in {toLanguage} from category:{toCategory} 
            and Difficulty:{difficultyString} commonly used with the {fromLanguage} word {fromWord}.  
            Present the results as a plain text file with two comma-separated columns: word in {fromLanguage}, 
            word in {toLanguage} and nothing more.";

        findRelatedWordsGptQuery = FormatMultiLineLiteral(findRelatedWordsGptQuery);
        DebugMultiLineLog($"Query: {findRelatedWordsGptQuery}");
        String responseString = await callChatGpt(findRelatedWordsGptQuery);
        DebugMultiLineLog($"Response: {responseString}");

        if (responseString.Length > 0)
        {
            var resultsDataTable = ConvertToTupleList(responseString);
            return resultsDataTable;
        }

        return null;
    }
}
