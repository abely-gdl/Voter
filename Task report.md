# Report

## Step #1 - app idea

**Tool:** ChatGPT

<details>
<summary>Prompt</summary>

"The idea is up to you (e.g. a small UI + API app), but it should include at least: Several pages and components Routing A few different layouts (e.g. main layout, admin layout, etc.) I need to create a small project via Github copilot to finish vibe-coding coures within my company. The requirements are above. Give me several ideas that I can choose"

</details>

**Results:** Several good ideas. I chose the voting app and requested several business use-cases that were used as prompts for step #3.

## Step #2 - instructions

**Tool:** GitHub Copilot + Claude Sonnet 4.5 + Agent mode

<details>
<summary>Prompts</summary>

- "Make a git repo within this folder. I will create a React+Net solution within this folder with tests. Create basic folder template, initialize git repository"
- "Create instructions for Github Copilot for this repo. We will use ASP.NET for back-end and React for front-end"
- "Please also adjust instrucitons with the following:
  - you can suggest ideas for use-cases, using the app. like being a PO/BA
  - for the changes don't give big explanations. If I need the details, I'll ask.
  - and don't overcomplicate things"

</details>

**Results:** Initialized the repo and folders, basic gitignore. I then had some auth problems, so I synced local and GitHub repos manually, but I guess by default it should work fine.

## Step #3 - Plan

**Tools:** GitHub Copilot + Claude Sonnet 4.5 + Plan mode + Agent mode

<details>
<summary>Prompts</summary>

- "I want to build a simple web app that will use .NET api and React to build the following system. The idea is not final, you can suggest changes or improvements. Generally I need a working solution.

  It must include at least:
  - Several pages and components
  - Routing
  - A few different layouts (e.g. main layout, admin layout, etc.)

  The idea: Voting Boards
  
  Goal: A web app where users can:
  - browse "boards" (poll spaces),
  - submit suggestions to a board,
  - vote on suggestions,
  - view results.

  Admins can:
  - create/edit boards (basic settings),
  - approve/reject suggestions (optional toggle),
  - close/open voting.

  Roles:
  
  User:
  - View boards
  - View board details + suggestion list
  - Add suggestion (if suggestions are open)
  - Vote (single or multi)
  - See results (live or after close)

  Admin:
  - Create/update boards
  - Configure voting rules (single vs multi, max votes)
  - Toggle "suggestions require approval"
  - Approves/rejects suggestions
  - Close/open voting

  Auth can be fake:
  hardcoded "admin mode" toggle, or a /login that sets a flag in local storage."

- "Let's adjust the plan/instructions with the new idea. Instead of fake login we'll just create a default admin user with default password. Create just two roles: Admin-User within our DB/models. Just keep it simple but still user need to type correct credentials"

</details>

**Results:** He created a good plan with several steps and started following it. Also, I saved the plan as a prompt, but after the request to adjust the plan, he didn't adjust the previous one but created a new file. Then he started implementation. Actually, it was like the Pareto law in action. He created 80% of functionality almost with no issues. I just answered questions like "Do you want to proceed?" No tests at this point.

## Step #4 - Bug fixing

**Tools:** GitHub Copilot + Claude Sonnet 4.5 + Agent mode

<details>
<summary>Prompts</summary>

- "When I tried to add a suggestion I've got 404"

- "On the main page I see 0 for counts despite I've created the board and vote for it. Also please remind me login and password for admin"

- "There's still an error. I want you to go through the API and UI contracts and mapper. You should fix the following:
  - UI contracts and API should be in sync
  - mapping should be correct
  - I don't like comparison to null for each field of the model in the service. Add 'Required' attribute to required fields. Other fields may be nullable then
  - if there are a lot of sequential validation checks move it to a separate private method"

- "BE instructions:
  - use arrow-styled methods where possible
  - move interfaces to a separate folder
  - rename ToggleSuggestions to ToggleSuggestionsStatus. Make it in similar places. Don't hide the logic from names
  - don't omit access modifiers. use them explicitly everywhere
  - considering I store data in sqlite, should sqlite files be in the repo or should I move it to gitignore?
  - don't store multiple models/dtos in the same file. Create separate folders as groups but place the classes in its own files
  
  FE instructions:
  - there's an error on BoardDetailPage. Also there's a redundant import, redundant variables. Check all the UI files to remove redundant imports/code and fix errors
  - I think during the npm run I saw warnings about deprecated packages. validate packages and update where needed. If not possible, explain why
  - split types into different files"

- "I don't see any changes. Maybe need to restart server?"

- "Still nothing. Maybe I can restart service?"

- "Logged in as admin I don't see pending suggestions from other users"

</details>

**Results:** That's where Pareto's 20% came from. I had to explain use cases that failed, he tried to fix them, sometimes he tried to run the app from the root folder (but csproj is placed in the inner one), he didn't restart the server so the changes weren't applied. Also, after code review I found several non-standard things like "placing several classes in a single file." So at this step I spent about 30 percent of the whole time. It wasn't pleasant at all. You never know what he decides to break while fixing.

## Step #5 - Tests

**Tools:** GitHub Copilot + Claude Sonnet 4.5 + Agent mode

<details>
<summary>Prompts</summary>

- "Now write the tests for BE"

- "I want you to fix BE tests. You're writing them"

</details>

**Results:** He managed to do it, but it didn't look like he understood what he was doing. He tried 10+ attempts to get the tests to compile. Each time there were some mistakes in contracts and models. I didn't launch test writing for FE as it already took too much time and nerves. He made some tests, and they ran successfully. Maybe I should have provided controllers/services as context for him manually, because all the time he was in the same chat. I guess when the context overflows he just condenses it into a new context session.

## Step #6 - Wrapping-up

**Tools:** GitHub Copilot + Claude Sonnet 4.5 + Agent mode

**Results:** Fixed some ESLint errors, updated README, created the report.

## Anomalies

- Initially he suggested to use .NET 8 with ASP.NET Core 10 (version mismatch)
- A lot of version mismatches with NuGet packages. Then he fixed it within several iterations
- Created a new file for Plan instead of adjusting the old one
- When the context was full, 'summarizing the context' was a very long operation, which was kind of irritating
- Constantly tried to run BE from the wrong folder
- His quote: "The test structure provides comprehensive coverage. To make them work, you'll need to align them with your actual service/controller signatures, or I can help fix specific test files if you'd like." It was like 'I've created something that doesn't compile, go fix it'. Kind of annoying. And as I mentioned, the whole 'write BE tests' process wasn't smooth. But yeah, maybe I should have given him context precisely
- Also, after the UI was finished, almost every UI component had unnecessary imports or variables. And of course, compile errors. Fixed after I told him, but I had to look through them all




