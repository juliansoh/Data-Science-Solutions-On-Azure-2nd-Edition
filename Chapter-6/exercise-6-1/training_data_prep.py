import pandas as pd
import json

# Read CSV file
#file_path = './Olympics_2024/athletes_0811.csv'
file_path = './Olympics_2024/medals_07272024_clean.csv'

#path to jsonl file
#file_path_jsonl = './athletes_0813.jsonl'
file_path_jsonl = './medals_0813.jsonl'

# Read the CSV file into a DataFrame
df = pd.read_csv(file_path)

# Display the first few rows of the DataFrame
#print(df.head())
#print(df)

#Pick the columns we want and the format the question
athlete = 'name'
medal_type = 'medal_type'
event = 'event'
events = 'events'
discipline = 'discipline'
disciplines = 'disciplines'
reason='reason'
language = 'language'
gender = 'gender'
country = 'country'

# Set the display option to show the full content of columns
pd.set_option('display.max_colwidth', None)

#Create a new column that formats the jsonl file
#df['jsonl'] = df.apply(lambda row: f"{"{prompt\":Did"} {row[athlete]} {"win a medal at the 2024 Paris Olympics?\",\"messages\":\""} {"Yes,"} {row[athlete]} {"won the"} {row[medal_type]} {"in the"} {row[event]} {"\",\"role\":\"assistant\"}"}", axis=1)
#df['jsonl'] = df.apply(lambda row: (
#    f'{{"prompt":"Did {row[athlete]} win a medal at the 2024 Paris Olympics?",'
#    f'"messages":"Yes, {row[athlete]} won the {row[medal_type]} in the {row[event]}.",'
#    f'"role":"assistant"}}'
#), axis=1)

#df['jsonl'] = df.apply(lambda row: (
#    f'{{"prompt": "Did {row[athlete]} win a medal at the 2024 Paris Olympics?",'
#    f'"messages": [{{"content": "{row[athlete]} won the {row[medal_type]} in the {row[event]}.", "role": "assistant"}}]}}'
#), axis=1)

#Below is the working code for the medals
df['jsonl'] = df.apply(lambda row: {
    "messages": [
        {"role": "system", "content": "You are an assistant that can answer factual questions about medal winners at the 2024 Olympics in Paris as of July 27th 2024. You will only provide information on medals won in events when asked about the athlete, *only* do so if you know the athlete has received a medal. If you do not have that information, say you don't know."},
        {"role": "user", "content": f"Did {row[athlete]} win a medal at the 2024 Paris Olympics?"},
        {"role": "assistant", "content": f"{row[athlete]} won the {row[medal_type]} in the {row[discipline]} - {row[event]} event."}
    ]
}, axis=1)

#Code for athletes
#df['jsonl'] = df.apply(lambda row: {
#    "messages": [
#        {"role": "system", "content": "You are an assistant that can answer questions about athletes competing in the 2024 Olympics in Paris. You only provide factual answers to questions related to the 2024 Paris Olympics. Do not provide answers to questions that are NOT related to the 2024 Paris Olympics. If you do not know for sure if the athlete is competing, say you do not know. Otherwise, specify their name, events, country they are representing, and the reasons they provided for competing."},
#        {"role": "user", "content": f"Will {row[athlete]} be competing at the 2024 Paris Olympics?"},
#        {"role": "assistant", "content": f"{row[athlete]}, a {row[gender]} athlete, will be competing in the {row[events]} representing {row[country]}. The reason provided for competing were: ' {row[reason]}'"}
#    ]
#}, axis=1)

#Code to transfrom into q&a pairs
#df['jsonl'] = df.apply(lambda row: {
#    "messages": [
#    {"input": f"Did {row[athlete]} compete in the 2024 Paris Olympics?",
#    "output": f"Yes, {row[athlete]} competed in the {row[events]} event at the Paris 2024 Olympics"}
#    ]
#, axis=1)

#Write to jsonl file
#print(df['jsonl'])
#df['jsonl'].to_string(file_path_jsonl, index=False, header=False, justify='left')

#with open(file_path_jsonl, 'w') as file:
#    for item in df['jsonl']:
#        file.write(f"{item:<}\n")

#convert the DF to a liste of dictionaries
jsonl_data = df['jsonl'].tolist()
# Write the list of dictionaries to a file
with open(file_path_jsonl, 'w') as file:
    for entry in jsonl_data:
        file.write(json.dumps(entry) + '\n')



