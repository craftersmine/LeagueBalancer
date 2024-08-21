name: Bug Report
description: File a bug report here.
title: "[Bug]: "
labels: ["bug"]
assignees:
    - craftersmine
body:
    - type: markdown
      attributes:
        value: |
            Thanks for taking the time to fill out this bug report!
    - type: textarea
      id: describe-the-bug
      attributes:
        label: What happened?
        description: Describe the bug, tell what happened, and what did you expect to happen.
        placeholder: A clear and concise description of what the bug is.
        value: "A clear and concise description of what the bug is."
      validations:
        required: true
    - type: textarea
      id: how-reproduce
      attributes:
        label: How to reproduce the issue?
        description: Describe the steps to reproduce this issue, step by step, as much information as possible.
        placeholder: |
            "1. Go to '...'"
            "2. Click on '....'"
            "3. Scroll down to '....'"
            "4. See error"
      validations:
        required: true
    - type: textarea
      id: expected-behaviour
      attributes:
        label: What did you expect to happen?
        description: Describe the behaviour of what did you expect to happen.
        placeholder: A clear and concise description of what you expected to happen.
        value: "A clear and concise description of what you expected to happen."
      validations:
        required: true
    - type: textarea
      id: technical-info
      attributes:
        label: Technical info
        description: Describe your system environment in which you using the app (OS, .NET version, Application verison, etc.).
        placeholder:  |
            " - OS: [e.g. Windows 10 Home]"
            " - .NET version [e.g. v6.0]"
            " - Application version: [e.g. 1.3.1]"
      validations:
        required: false
    - type: textarea
      id: additional-context
      attributes:
        label: Any other information to tell? Write it hereю
        description: Add any other context about the problem here.
        placeholder: "Add any other context about the problem here."
      validations:
        required: false
