#from together import Together
import together
from flask import Flask,render_template, request
import os
from dotenv import load_dotenv

load_dotenv()
client = together.Together(api_key=os.getenv("TOGETHER_API_KEY"))
app = Flask(__name__)
prompt = "generate a unity game that look like temple run with a sphere and a infinite road loop with obstacles, i want all the objects to be done wth code i dont want touch the ui interface"

response = client.chat.completions.create(
    model="deepseek-ai/DeepSeek-R1-Distill-Llama-70B-free",
    messages=[
      {
        "role": "user",
        "content": f"{prompt}"
      }
    ]
)
output = response.choices[0].message.content
f=False
tmp=0
clean_output =""
for i in range(len(output)):
    if output[i]=="`" and output[i+1]=="`" and output[i+2]=="`":
        if f==False:  
            i+=10
            tmp=i
            f=True
        else : 
            clean_output+=output[tmp:i-1]
            f=False
print(output)


response1 = client.chat.completions.create(
    model="meta-llama/Llama-3.3-70B-Instruct-Turbo-Free",
    messages=[
      {
        "role": "user",
        "content": f"this is the prompt that i gave previously so i can have a code generated : {prompt} can you check the logic and optimize if there some issues or optimizations it can be done, here is the code generated : {clean_output}"
      }
    ]
)

output1 = response1.choices[0].message.content
f=False
tmp=0
clean_output1 =""
for i in range(len(output1)):
    if output1[i]=="`" and output1[i+1]=="`" and output1[i+2]=="`":
        if f==False:  
            i+=10
            tmp=i
            f=True
        else : 
            clean_output1+=output1[tmp:i-1]
            f=False

file = open("test.cs", "w")
file.write(clean_output1)
file.close()
print(output1)
@app.route("/")
def home():
  return render_template("index.html",code=clean_output1)

if __name__ == "__main__":
    app.run(debug=True)