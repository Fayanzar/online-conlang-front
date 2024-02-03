module OnlineConlangFront.Shared

open Lit

let styles = [
    css $"""
        table, th, td {{
            border: 1px solid rgb(30, 30, 33);
            border-collapse: collapse;
            border-radius: 0.25m;
            padding: 5px 5px 5px 5px;
            margin: 5px 5px 5px 5px;
        }}

        table tr:nth-child(even) {{
            background-color: rgb(42, 42, 48);;
        }}

        table tr:nth-child(odd) {{
            background-color: rgb(48, 48, 56);
        }}

        td {{
            background-color: inherit;
        }}

        th {{
            background-color: rgb(33, 33, 36);
        }}

        select {{
            /* Reset Select */
            appearance: none;
            outline: 10px red;
            border: 0;
            box-shadow: none;
            /* Personalize */
            flex: 1;
            font: inherit;
            padding: 0rem 2.5rem 0rem 0.5rem;
            color: #fff;
            background-color: var(--darkgray);
            background-image: none;
            cursor: pointer;
        }}
        /* Remove IE arrow */
        select::-ms-expand {{
            display: none;
        }}
        /* Custom Select wrapper */
        .select {{
            position: relative;
            height: 2rem;
            display: inline-flex;
            border-radius: .25em;
            overflow: hidden;
            margin-top: 2px;
            margin-bottom: 2px;
        }}
        /* Arrow */
        .select::after {{
            content: '\25BC';
            color: white;
            position: absolute;
            top: 0;
            right: 0;
            padding: 0.4rem 0.5rem 0.6rem 0.5rem;
            background-color: var(--arrow-bg);
            transition: .25s all ease;
            pointer-events: none;
        }}
        /* Transition */
        .select:hover::after {{
            color: #f39c12;
        }}

        button {{
            appearance: none;
            outline: 0;
            border: 0;
            color: white;
            box-shadow: none;
            font: inherit;
            height: 2rem;
            border-radius: .25em;
            margin-top: 2px;
            margin-bottom: 2px;
            padding: 0rem 0.5rem 0rem 0.5rem;
            cursor: pointer;
            background-color: var(--darkgray);
        }}

        button:hover {{
            background-color: var(--arrow-bg);
        }}

        button.rule-delete {{
            margin-right: 10px;
        }}

        input:not([type='checkbox']) {{
            appearance: none;
            outline: 0;
            border: 0;
            color: white;
            font: inherit;
            border-radius: .25em;
            margin: 0;
            padding: 0rem 0.5rem 0rem 0.5rem;
            height: 2rem;
            background-color: var(--darkgray);
        }}

        input.t-input {{
            outline: solid var(--input-outline);
            margin-left: 5px;
        }}

        input.t-output {{
            outline: solid var(--output-outline);
            margin-right: 5px;
        }}

        input.suffix {{
            outline: solid var(--suffix-outline);
        }}

        input.prefix {{
            outline: solid var(--prefix-outline);
            margin-left: 5px;
        }}

        input.infix {{
            outline: solid var(--infix-outline);
        }}

        input.infix-pos {{
            outline: solid var(--infix-pos-outline);
        }}

        div.transformation {{
            background-color: rgba(0, 0, 117, 0.1);
            border-radius: .25em;
            border-top: 1px solid rgba(0, 0, 117, 0.2);
            border-bottom: 1px solid rgba(0, 0, 117, 0.2);
        }}

        div.suffix {{
            background-color: rgba(117, 57, 0, 0.1);
            border-radius: .25em;
            border-top: 1px solid rgba(117, 57, 0, 0.2);
            border-bottom: 1px solid rgba(117, 57, 0, 0.2);
        }}

        div.prefix {{
            background-color: rgba(0, 117, 0, 0.1);
            border-radius: .25em;
            border-top: 1px solid rgba(0, 117, 0, 0.2);
            border-bottom: 1px solid rgba(0, 117, 0, 0.2);
        }}

        div.infix {{
            display: flex;
            background-color: rgba(70, 0, 117, 0.1);
            border-radius: .25em;
            border-top: 1px solid rgba(70, 0, 117, 0.2);
            border-bottom: 1px solid rgba(70, 0, 117, 0.2);
            height: 4.67rem;
            align-items: center;
        }}
    """;
    css $"""
        div.apply-multiple-checkbox {{
            display: inline-block;
        }}

        .apply-multiple-checkbox .tgl {{
            display: none;
        }}
        .apply-multiple-checkbox .tgl,
        .apply-multiple-checkbox .tgl:after,
        .apply-multiple-checkbox .tgl:before,
        .apply-multiple-checkbox .tgl *,
        .apply-multiple-checkbox .tgl *:after,
        .apply-multiple-checkbox .tgl *:before,
        .apply-multiple-checkbox .tgl + .tgl-btn {{
            box-sizing: border-box;
        }}
        .apply-multiple-checkbox .tgl::-moz-selection,
        .apply-multiple-checkbox .tgl:after::-moz-selection,
        .apply-multiple-checkbox .tgl:before::-moz-selection,
        .apply-multiple-checkbox .tgl *::-moz-selection,
        .apply-multiple-checkbox .tgl *:after::-moz-selection,
        .apply-multiple-checkbox .tgl *:before::-moz-selection,
        .apply-multiple-checkbox .tgl + .tgl-btn::-moz-selection,
        .apply-multiple-checkbox .tgl::selection,
        .apply-multiple-checkbox .tgl:after::selection,
        .apply-multiple-checkbox .tgl:before::selection,
        .apply-multiple-checkbox .tgl *::selection,
        .apply-multiple-checkbox .tgl *:after::selection,
        .apply-multiple-checkbox .tgl *:before::selection,
        .apply-multiple-checkbox .tgl + .tgl-btn::selection {{
            background: none;
        }}
        .apply-multiple-checkbox .tgl + .tgl-btn {{
            outline: 2px solid var(--font-color);
            display: block;
            width: 3em;
            height: 1.5em;
            position: relative;
            margin-bottom: -0.33rem;
            margin-right: 3px;
            margin-left: 3px;
            cursor: pointer;
            -webkit-user-select: none;
            -moz-user-select: none;
                -ms-user-select: none;
                    user-select: none;
        }}
        .apply-multiple-checkbox .tgl + .tgl-btn:after,
        .apply-multiple-checkbox .tgl + .tgl-btn:before {{
            position: relative;
            display: block;
            content: "";
            width: 50%%;
            height: 100%%;
        }}
        .apply-multiple-checkbox .tgl + .tgl-btn:after {{
            left: 0;
        }}
        .apply-multiple-checkbox .tgl + .tgl-btn:before {{
            display: none;
        }}
        .apply-multiple-checkbox .tgl:checked + .tgl-btn:after {{
            left: 50%%;
        }}

        .apply-multiple-checkbox .tgl-light + .tgl-btn {{
            background: var(--darkgray);
            border-radius: 0.5em;
            padding: 2px;
            transition: all 0.4s ease;
        }}
        .apply-multiple-checkbox .tgl-light + .tgl-btn:after {{
            border-radius: 25%%;
            background: lightgrey;
            transition: all 0.2s ease;
        }}
        .apply-multiple-checkbox .tgl-light:checked + .tgl-btn {{
            background: #47204d;
        }}
    """
    ]
