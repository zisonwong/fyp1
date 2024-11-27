<%@ Page Title="" Language="C#" MasterPageFile="~/NavFooter.Master" AutoEventWireup="true" CodeBehind="clientHome.aspx.cs" Inherits="fyp1.Client.clientHome" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../CSS/home.css" rel="stylesheet" />
    <link href="../CSS/footer.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <form id="form1" runat="server">
        <section class="hero">
            <div class="hero-text">
                <h1>Welcome to Our Hospital</h1>
                <p>Providing compassionate care and the latest medical advancements</p>
                <a href="BranchDoctorSelection.aspx" class="btn type1">Hover Me
                <tb>
                    &rarr;</a>
            </div>
        </section>

        <section id="services">
            <h2>Our Main Services</h2>
            <div class="servicesList">
                <div class="service-card">
                    <div class="card-inner">
                        <div class="card-front">
                            <div class="card-details">
                                <p class="text-title">Cardiology</p>
                                <p class="text-body">Comprehensive heart care and treatment.</p>
                            </div>
                            <button class="card-button more-info">More info</button>
                        </div>
                        <div class="card-back">
                            <p>Cardiology is the branch of medicine that deals with diseases and abnormalities of the heart.</p>
                            <button class="card-button back-button">Back</button>
                        </div>
                    </div>
                </div>

                <div class="service-card">
                    <div class="card-inner">
                        <div class="card-front">
                            <div class="card-details">
                                <p class="text-title">Maternity</p>
                                <p class="text-body">Expert care for mothers and babies.</p>
                            </div>
                            <button class="card-button more-info">More info</button>
                        </div>
                        <div class="card-back">
                            <p>Maternity services focus on the care of women during pregnancy and childbirth.</p>
                            <button class="card-button back-button">Back</button>
                        </div>
                    </div>
                </div>

                <div class="service-card">
                    <div class="card-inner">
                        <div class="card-front">
                            <div class="card-details">
                                <p class="text-title">Psychology</p>
                                <p class="text-body">the scientific study of mind and behavior</p>
                            </div>
                            <button class="card-button more-info">More info</button>
                        </div>
                        <div class="card-back">
                            <p>Psychology involves the study of the mind and behavior, offering various therapeutic approaches.</p>
                            <button class="card-button back-button">Back</button>
                        </div>
                    </div>
                </div>
            </div>
        </section>


        <section class="aboutUsSection">
            <h1 class="aboutUsheader">ABOUT US </h1>
            <div class="aboutUs">
                <img src="../Images/aboutUs1.jpg" />
                <div class="imageBanner">
                    <h5>Trinity: Our Mission and History</h5>
                    <br>
                    <br>
                    <p>
                        Welcome to Trinity Medical Center, where our mission is to provide exceptional healthcare to our community with compassion and excellence. Founded in 1990, our hospital has been a beacon of health and healing, dedicated to improving the lives of our patients.
                    </p>
                </div>
            </div>

            <div class="aboutUs">
                <div class="imageBanner">
                    <h5>Personalized and Advanced Care </h5>
                    <br>
                    <br>

                    <p>
                        At Trinity Medical Center, we believe in combining the latest medical advancements with personalized care. Our team of highly skilled doctors, nurses, and support staff work tirelessly to ensure that each patient receives the best possible treatment in a caring and comfortable environment.
                    </p>
                </div>
                <img src="../Images/aboutUs2.jpg" />
            </div>
            <div class="aboutUs">
                <img src="../Images/aboutUs3.png" />
                <div class="imageBanner">
                    <h5>State-of-the-Art Facilities
                    <br>
                        <br>
                    </h5>
                    <p>
                        Our state-of-the-art facilities are equipped with the latest technology to provide a wide range of services, from routine check-ups to complex surgeries. We are proud to offer specialized departments in cardiology, maternity, psychology, and more, ensuring comprehensive care for all your health needs.
                    </p>
                </div>

            </div>
        </section>

        <section id="resources">
            <h2>Patient Resources</h2>
            <ul>
                <li class="resource-item">
                    <a href="../Client/clientProfile.aspx">
                        <div class="resource-content">
                            <h3>Patient Portal</h3>
                            <p>Access your medical records and manage your appointments online.</p>
                        </div>
                    </a>
                </li>
                <li class="resource-item">
                    <a href="#">
                        <div class="resource-content">
                            <h3>Insurance Information</h3>
                            <p>Get information on accepted insurance plans and coverage details.</p>
                        </div>
                    </a>
                </li>
                <li class="resource-item">
                    <a href="#">
                        <div class="resource-content">
                            <h3>FAQs</h3>
                            <p>Find answers to common questions about our services and policies.</p>
                        </div>
                    </a>
                </li>
            </ul>
        </section>


        <section id="testimonials">
            <h2>What Our Patients Say</h2>
            <div class="testimonial-container">
                <div class="testimonial-card">
                    <p>"The care I received was outstanding!" - John Doe</p>
                    <div class="rating">
                        <input value="5" name="rate1" id="star5-1" type="radio" checked disabled>
                        <label title="text" for="star5-1"></label>
                        <input value="4" name="rate1" id="star4-1" type="radio" disabled>
                        <label title="text" for="star4-1"></label>
                        <input value="3" name="rate1" id="star3-1" type="radio" disabled>
                        <label title="text" for="star3-1"></label>
                        <input value="2" name="rate1" id="star2-1" type="radio" disabled>
                        <label title="text" for="star2-1"></label>
                        <input value="1" name="rate1" id="star1-1" type="radio" disabled>
                        <label title="text" for="star1-1"></label>
                    </div>
                </div>
                <div class="testimonial-card">
                    <p>"The staff was very friendly and professional." - Jane Smith</p>
                    <div class="rating">
                        <input value="5" name="rate2" id="star5-2" type="radio" checked disabled>
                        <label title="text" for="star5-2"></label>
                        <input value="4" name="rate2" id="star4-2" type="radio" disabled>
                        <label title="text" for="star4-2"></label>
                        <input value="3" name="rate2" id="star3-2" type="radio" disabled>
                        <label title="text" for="star3-2"></label>
                        <input value="2" name="rate2" id="star2-2" type="radio" disabled>
                        <label title="text" for="star2-2"></label>
                        <input value="1" name="rate2" id="star1-2" type="radio" disabled>
                        <label title="text" for="star1-2"></label>
                    </div>
                </div>
                <div class="testimonial-card">
                    <p>"I felt well cared for throughout my treatment." - Sam Brown</p>
                    <div class="rating">
                        <input value="5" name="rate3" id="star5-3" type="radio" checked disabled>
                        <label title="text" for="star5-3"></label>
                        <input value="4" name="rate3" id="star4-3" type="radio" disabled>
                        <label title="text" for="star4-3"></label>
                        <input value="3" name="rate3" id="star3-3" type="radio" disabled>
                        <label title="text" for="star3-3"></label>
                        <input value="2" name="rate3" id="star2-3" type="radio" disabled>
                        <label title="text" for="star2-3"></label>
                        <input value="1" name="rate3" id="star1-3" type="radio" disabled />
                        <label title="text" for="star1-3"></label>
                    </div>
                </div>
            </div>
        </section>

        <section id="Highlights">
            <h2>Highlights of Our Hospital</h2>
            <div class="homeImageSliderContainer">

                <div class="sliderImageList">
                    <div class="sliderImageItem active">
                        <img src="../Images/sliderImage2.jpg" />
                    </div>
                    <div class="sliderImageItem">
                        <img src="../Images/sliderImage3.jpg" />
                    </div>
                    <div class="sliderImageItem">
                        <img src="../Images/sliderImage4.jpg" />
                    </div>
                    <div class="sliderImageItem">
                        <img src="../Images/sliderImage5.jpg" />
                    </div>
                </div>

                <!-- button prev and next -->
                <div class="buttons">
                    <asp:Button class="prev" runat="server" ID="PrevButton" OnClick="PrevBtnClick" Text="<" OnClientClick="return false;"></asp:Button>
                    <asp:Button class="next" runat="server" ID="NextButton" OnClick="NextBtnClick" OnClientClick="return false;" Text=">"></asp:Button>
                </div>

                <!-- dots -->
                <div class="dots"></div>
            </div>
        </section>

        <script>
            document.addEventListener("DOMContentLoaded", function () {
                const sliderImageItems = document.querySelectorAll(".sliderImageItem");
                const dotsContainer = document.querySelector(".dots");

                // Generate dots dynamically based on the number of slider images
                sliderImageItems.forEach((item, index) => {
                    const dot = document.createElement("span");
                    dot.classList.add("dot");
                    if (index === 0) {
                        dot.classList.add("active");
                    }
                    dot.dataset.index = index;
                    dotsContainer.appendChild(dot);
                });

                const dots = document.querySelectorAll(".dot");
                let currentIndex = 0;
                let intervalId;

                // Function to update the slider to the selected image
                function updateSlider(index) {
                    sliderImageItems[currentIndex].classList.remove("active");
                    dots[currentIndex].classList.remove("active");
                    sliderImageItems[index].classList.add("active");
                    dots[index].classList.add("active");
                    currentIndex = index;
                    reloadSlider();
                }

                // Function to move the slider to the active item
                function reloadSlider() {
                    const list = document.querySelector('.homeImageSliderContainer .sliderImageList');
                    const activeItem = document.querySelector('.homeImageSliderContainer .sliderImageItem.active');
                    const checkLeft = activeItem.offsetLeft;
                }

                // Function to move to the next image
                function nextImage() {
                    let newIndex = currentIndex + 1;
                    if (newIndex >= sliderImageItems.length) {
                        newIndex = 0;
                    }
                    updateSlider(newIndex);
                }

                // Function to move to the previous image
                function prevImage() {
                    let newIndex = currentIndex - 1;
                    if (newIndex < 0) {
                        newIndex = sliderImageItems.length - 1;
                    }
                    updateSlider(newIndex);
                }

                // Event listeners for the previous and next buttons
                document.querySelector(".prev").addEventListener("click", () => {
                    prevImage();
                    resetInterval();
                });

                document.querySelector(".next").addEventListener("click", () => {
                    nextImage();
                    resetInterval();
                });

                // Event listeners for the dots
                dots.forEach(dot => {
                    dot.addEventListener("click", () => {
                        updateSlider(parseInt(dot.dataset.index));
                        resetInterval();
                    });
                });

                // Function to start the automatic slider
                function startInterval() {
                    intervalId = setInterval(nextImage, 5000);
                }

                // Function to reset the automatic slider interval
                function resetInterval() {
                    clearInterval(intervalId);
                    startInterval();
                }

                // Start the automatic slider
                startInterval();

                // Reload slider position on window resize
                window.addEventListener('resize', reloadSlider);
            });

            // Select all service cards
            document.querySelectorAll('.service-card').forEach(card => {
                // Get the buttons for more info and back
                const moreInfoButton = card.querySelector('.more-info');
                const backButton = card.querySelector('.back-button');

                // Add event listener for the "More info" button
                moreInfoButton.addEventListener('click', (e) => {
                    e.preventDefault(); // Prevent the default action of the button
                    card.querySelector('.card-inner').classList.add('flipped'); // Add the flipped class
                });

                // Add event listener for the "Back" button
                backButton.addEventListener('click', (e) => {
                    e.preventDefault(); // Prevent the default action of the button
                    card.querySelector('.card-inner').classList.remove('flipped'); // Remove the flipped class
                });
            });



        </script>
        </form>
</asp:Content>

