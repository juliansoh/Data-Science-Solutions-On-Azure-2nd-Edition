import pandas as pd
import json

# Read CSV file
file_path = './data/raw/medals.csv'

#path to jsonl file
file_path_jsonl = './data/prepped_data_for_training/medals.jsonl'

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

#Assemble the desired columns into data format for fine-tuning (jsonl format)
#Sample format:
#{"messages": [{"role": "system", "content": "You are an Xbox customer support agent whose primary goal is to help users with issues they are experiencing with their Xbox devices. You are friendly and concise. You only provide factual answers to queries, and do not provide answers that are not related to Xbox."}, {"role": "user", "content": "Is Xbox better than PlayStation?"}, {"role": "assistant", "content": "I apologize, but I cannot provide personal opinions. My primary job is to assist you with any issues related to your Xbox device. Do you have any Xbox-related issues that need addressing?"}]}
#{"messages": [{"role": "system", "content": "You are an Xbox customer support agent whose primary goal is to help users with issues they are experiencing with their Xbox devices. You are friendly and concise. You only provide factual answers to queries, and do not provide answers that are not related to Xbox."}, {"role": "user", "content": "Can I play PC games on my Xbox?"}, {"role": "assistant", "content": "While Xbox consoles don't natively support PC games, they can play an array of Xbox games, many of which are also available on PC. Can I assist you with finding games compatible with your Xbox device?"}]}
#{"messages": [{"role": "system", "content": "You are an Xbox customer support agent whose primary goal is to help users with issues they are experiencing with their Xbox devices. You are friendly and concise. You only provide factual answers to queries, and do not provide answers that are not related to Xbox."}, {"role": "user", "content": "I'm having trouble connecting my Xbox to the Wi-Fi."}, {"role": "assistant", "content": "No worries, let's go through the network settings on your Xbox. Can you please tell me what happens when you try to connect it to the Wi-Fi?"}]}
#Read https://learn.microsoft.com/en-us/azure/ai-studio/how-to/fine-tune-phi-3?tabs=phi-3-mini
df['jsonl'] = df.apply(lambda row: {
    "messages": [
        {"role": "system", "content": "You are an assistant that can answer factual questions about medal winners at the 2024 Olympics in Paris as of July 27th 2024. You will only provide information on medals won in events when asked about the athlete, *only* do so if you know the athlete has received a medal. If you do not have that information, say you don't know."},
        {"role": "user", "content": f"Did {row[athlete]} win a medal at the 2024 Paris Olympics?"},
        {"role": "assistant", "content": f"{row[athlete]} won the {row[medal_type]} in the {row[discipline]} - {row[event]} event."}
    ]
}, axis=1)

#Optional/alternate dataset
#Code for athletes
#df['jsonl'] = df.apply(lambda row: {
#    "messages": [
#        {"role": "system", "content": "You are an assistant that can answer questions about athletes competing in the 2024 Olympics in Paris. You only provide factual answers to questions related to the 2024 Paris Olympics. Do not provide answers to questions that are NOT related to the 2024 Paris Olympics. If you do not know for sure if the athlete is competing, say you do not know. Otherwise, specify their name, events, country they are representing, and the reasons they provided for competing."},
#        {"role": "user", "content": f"Will {row[athlete]} be competing at the 2024 Paris Olympics?"},
#        {"role": "assistant", "content": f"{row[athlete]}, a {row[gender]} athlete, will be competing in the {row[events]} representing {row[country]}. The reason provided for competing were: ' {row[reason]}'"}
#    ]
#}, axis=1)

#convert the DF to a liste of dictionaries
jsonl_data = df['jsonl'].tolist()
# Write the list of dictionaries to a file
with open(file_path_jsonl, 'w') as file:
    for entry in jsonl_data:
        file.write(json.dumps(entry) + '\n')



