
$(document).ready(function () {

    var categoriesDesciptions = {
        "gamingComputers": "Powerful desktops built for gaming, with high-performance CPUs and GPUs, fast storage, and advanced cooling systems for smooth and fast gameplay",
        "gamingLaptops": "Portable computers designed for gaming, with high-performance CPUs and GPUs, fast storage, and advanced cooling systems for smooth and fast gameplay on the go.",
        "gamingMonitors": "High-performance displays designed for gaming, featuring fast refresh rates and low input lag for a smooth and responsive experience, with vibrant colors and high resolutions.",
        "gamingChairs": "Ergonomic chairs designed for gamers, with adjustable features such as lumbar support, armrests, and recline, for comfortable and healthy gaming posture during long sessions.",
        "gamingMB": "The main circuit board in a computer, responsible for connecting and controlling other components, with features such as multiple PCI-E slots, M.2 connectors, and RGB lighting for customization.",
        "gamingCPU": "The central processing unit, responsible for executing instructions and performing calculations in a computer, with high-core-count and high-clock-speed options available for demanding applications such as gaming.",
        "gamingGPU": "The graphics processing unit, responsible for rendering images and video in a computer, with high-end options featuring ray tracing and AI acceleration for immersive and realistic visuals.",
        "gamingPSU": "Provides power to a computer, with higher wattage options available for more powerful components, and features such as modular cabling and high efficiency ratings for clean and stable power delivery.",
        "gamingSSD": "Solid-state drives, faster and more reliable than traditional hard disk drives for storing data, with options available for high capacities and fast transfer speeds for demanding applications",
        "gamingHdd": "Hard disk drives, traditional and more affordable than SSDs for storing large amounts of data, with options available for high capacities and reliable performance for long-term storage.",
        "gamingMouse": " Input devices for moving a cursor on a computer screen, often with customizable buttons and DPI settings, with high-end options featuring advanced sensors and wireless connectivity for precise and fast tracking.",
        "gamingKeyboard": "Input devices for typing and executing commands, often with backlit keys and programmable macros, with high-end options featuring mechanical switches and customizable RGB lighting for a premium typing experience.",
        "gamingRAM": "Random access memory, temporary storage for data and program instructions for quick access, with options available for high capacities and fast speeds for demanding applications such as gaming.",
        "headSet": "Audio devices with headphones and a built-in microphone for communication and immersive sound during gaming, with high-end options featuring advanced audio drivers and noise-cancelling technology for an immersive and distraction-free experience.",
    };
    var productNames = {
        "gamingComputers": "Computers",
        "gamingMonitors": "Monitors",
        "gamingChairs": "Chairs",
        "gamingLaptops": "Laptops",
        "gamingMB": "motherBoards",
        "gamingCPU": "CPU",
        "gamingGPU": "GPU",
        "gamingPSU": "Power supplies",
        "gamingSSD": "SSD",
        "gamingHdd": "Hard disks",
        "gamingMouse": "Mouses",
        "gamingKeyboard": "KeyBoards",
        "gamingRAM": "RAM",
        "headSet": "HeadSet",
    }


    $(".categorieCard").click(function () {

        //change mainImg based on clicked card id
        var cardId = $(this).attr("id");
        $(".mainImg img").attr("src", "./imgs/" + cardId + ".png");
        //change name text based on clicked card
        var productName = productNames[cardId];
        $(".mainText h1").text(productName);
        //change description text based on clicked card
        var descriptionText = categoriesDesciptions[cardId];
        $(".mainText p").text(descriptionText);
        $(".getStarted").css("display", "flex");
        $(".getStarted a").attr("href", 'products?category=' + encodeURIComponent(productName));
       // window.location.href = 'products?category=' + encodeURIComponent(productName); // Redirect to product page with search parameter in URL
        
    });

});