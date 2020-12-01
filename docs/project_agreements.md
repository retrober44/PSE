# Team agreements and constraints
This file contains agreements made between the teams during a meeting on 11/30/2020 at 3:30 pm.

## Git

#### Branches  
Branches are to be named the following way:  
`type/your-corresponding-jira-task-name-separated-by-hypens`  
Only create feature branches, do not create personal branches in which work on multiple tasks.
Only work on one task per task branch.  
Types are, for example, `feature` or `bugfix`.  

Furthermore, we will be using the following branches:
- **master**  
This branch must be stable, and no changes may be pushed into this branch.  
It is meant to symbolize a production environment / release. 
- **staging**  
This branch does not have to be stable, but no changes may be pushed into this branch.  
We will merge `develop` into `staging` whenever we determine that we are ready for testing the current
state of `develop`.
- **develop**  
This branch does not have to be stable.  
Changes are supposed to be merged into this branch when they are done (check the definition of done).

The idea is that we create `feature`-branches, which is where we work on tasks.  
Once the tasks are developed, they will be merged into `develop`.  
As soon as we decide we are ready for a testing phase, which will be done continuously, `develop`
will be merged into `staging`.  
When readiness of the currently staged version is assured, `staging` will be merged into `master`.
`master` then symbolizes the release version of our product, as we would deliver it to customers
(who would probably be alpha and beta testers during development).

#### Designs
Commit designs of components, such as UML diagrams or textual explanations of how components are supposed
to work. This not only helps others understand your work, it is also helpful when writing the
project report.

#### Project report
LaTeX sources of the project report will also be versioned in git.

## Documenting features
Code should be documented according to the usual documentation practices
(i.e. JSDoc, XML documentation tags).
These documentation comments will be used to generate a documentation.
The generated documentation should be enough for others to quickly understand code, but most importantly
to quickly understand interfaces (i.e. how to call methods, APIs, how protocols work, etc.)

## Definition of done
A feature is done if 
- **Manual testing succeeds**
- **Sensible unit tests exist (if applicable) and pass**
    - This means unit tests should not just test that the program doesn't crash, they should also test results.
- **Code style was respected. Language specific style guides will be defined prior to the design milestone.**
For C#, examples of this include
    - Properties, fields, delegates, and events are camelCase
    - Classes and methods are PascalCase
    - Private fields do not have a `private` modifier
- **Code is documented according to documentation rules (comments!) and documentation is not outdated**
- **Acceptance criteria is satisfied**  
    An example of this might be
     
    `User story`: "As a user, I want to be able to leave a game session upon pressing
    the 'leave game' button.
    
    `Scenario`: User wants to leave a game session.
    User presses the 'leave game button'.  
    
    `Then` The client disconnects from the game. 
    
    (User stories and acceptance criteria will have to versioned in git so they are accessible to
    everyone.)
- **Code review is completed**  
Use pull requests and assign them to your teammates. You teammates may then review your code and,
upon their approval, it can be merged into the destination branch automatically.
Your teammates are also able to comment on specific changes you made.
